using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlogBackend.Blog;
using BlogBackend.Entities;
using BlogBackend.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客分类应用服务实现
    /// </summary>
    [Authorize]
    public class BlogCategoryAppService : ApplicationService, IBlogCategoryAppService
    {
        private readonly IBlogCategoryRepository _blogCategoryRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogCategoryAppService(
            IBlogCategoryRepository blogCategoryRepository,
            IBlogPostRepository blogPostRepository)
        {
            _blogCategoryRepository = blogCategoryRepository;
            _blogPostRepository = blogPostRepository;
        }

        public virtual async Task<PagedResultDto<BlogCategoryDto>> GetListAsync(GetBlogCategoryListDto input)
        {
            var totalCount = await _blogCategoryRepository.GetCountAsync();
            var categories = await _blogCategoryRepository.GetPagedListAsync(
                input.SkipCount, 
                input.MaxResultCount,
                input.Sorting ?? "Name");

            var categoryDtos = new List<BlogCategoryDto>();
            foreach (var category in categories)
            {
                var dto = ObjectMapper.Map<BlogCategory, BlogCategoryDto>(category);
                dto.PostCount = (int)await _blogPostRepository.GetCountAsync(categoryId: category.Id);
                categoryDtos.Add(dto);
            }

            return new PagedResultDto<BlogCategoryDto>(totalCount, categoryDtos);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCategoryBriefDto>> GetActiveCategoriesAsync()
        {
            var categories = await _blogCategoryRepository.GetListAsync();
            return categories
                .Where(c => c.IsActive)
                .Select(c => ObjectMapper.Map<BlogCategory, BlogCategoryBriefDto>(c))
                .ToList();
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCategoryTreeDto>> GetTreeAsync(bool onlyActive = true)
        {
            var categories = await _blogCategoryRepository.GetListAsync();
            if (onlyActive)
            {
                categories = categories.Where(c => c.IsActive).ToList();
            }

            var categoryDtos = categories.Select(c => ObjectMapper.Map<BlogCategory, BlogCategoryDto>(c)).ToList();

            // 填充文章数量
            foreach (var dto in categoryDtos)
            {
                dto.PostCount = (int)await _blogPostRepository.GetCountAsync(categoryId: dto.Id);
            }

            return BuildCategoryTree(categoryDtos);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCategoryBriefDto>> GetRootCategoriesAsync(bool onlyActive = true)
        {
            var categories = await _blogCategoryRepository.GetListAsync();
            var rootCategories = categories.Where(c => c.ParentId == null);
            
            if (onlyActive)
            {
                rootCategories = rootCategories.Where(c => c.IsActive);
            }

            return rootCategories
                .Select(c => ObjectMapper.Map<BlogCategory, BlogCategoryBriefDto>(c))
                .ToList();
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCategoryBriefDto>> GetChildCategoriesAsync(Guid parentId, bool onlyActive = true)
        {
            var categories = await _blogCategoryRepository.GetListAsync();
            var childCategories = categories.Where(c => c.ParentId == parentId);
            
            if (onlyActive)
            {
                childCategories = childCategories.Where(c => c.IsActive);
            }

            return childCategories
                .Select(c => ObjectMapper.Map<BlogCategory, BlogCategoryBriefDto>(c))
                .ToList();
        }

        public virtual async Task<BlogCategoryDto> GetAsync(Guid id)
        {
            var category = await _blogCategoryRepository.GetAsync(id);
            var dto = ObjectMapper.Map<BlogCategory, BlogCategoryDto>(category);
            dto.PostCount = (int)await _blogPostRepository.GetCountAsync(categoryId: id);
            return dto;
        }

        [AllowAnonymous]
        public virtual async Task<BlogCategoryDto> GetBySlugAsync(string slug)
        {
            var category = await _blogCategoryRepository.FindBySlugAsync(slug);
            if (category == null)
            {
                throw new EntityNotFoundException(typeof(BlogCategory), slug);
            }

            var dto = ObjectMapper.Map<BlogCategory, BlogCategoryDto>(category);
            dto.PostCount = (int)await _blogPostRepository.GetCountAsync(categoryId: category.Id);
            return dto;
        }

        public virtual async Task<BlogCategoryDto> CreateAsync(CreateBlogCategoryDto input)
        {
            // 检查Slug是否已存在
            if (await _blogCategoryRepository.SlugExistsAsync(input.Slug))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            // 如果有父分类，验证父分类是否存在
            if (input.ParentId.HasValue)
            {
                var parentCategory = await _blogCategoryRepository.FindAsync(input.ParentId.Value);
                if (parentCategory == null)
                {
                    throw new EntityNotFoundException(typeof(BlogCategory), input.ParentId.Value);
                }
            }

            var category = ObjectMapper.Map<CreateBlogCategoryDto, BlogCategory>(input);
            category = await _blogCategoryRepository.InsertAsync(category);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(category.Id);
        }

        public virtual async Task<BlogCategoryDto> UpdateAsync(Guid id, UpdateBlogCategoryDto input)
        {
            var category = await _blogCategoryRepository.GetAsync(id);

            // 检查Slug是否已存在（排除当前分类）
            if (category.Slug != input.Slug && await _blogCategoryRepository.SlugExistsAsync(input.Slug, id))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            // 如果有父分类，验证父分类是否存在且不能是自己或自己的子分类
            if (input.ParentId.HasValue)
            {
                if (input.ParentId.Value == id)
                {
                    throw new UserFriendlyException("Category cannot be its own parent.");
                }

                var parentCategory = await _blogCategoryRepository.FindAsync(input.ParentId.Value);
                if (parentCategory == null)
                {
                    throw new EntityNotFoundException(typeof(BlogCategory), input.ParentId.Value);
                }

                // 检查是否会形成循环引用
                if (await WouldCreateCycleAsync(id, input.ParentId.Value))
                {
                    throw new UserFriendlyException("Cannot set parent category as it would create a circular reference.");
                }
            }

            ObjectMapper.Map<UpdateBlogCategoryDto, BlogCategory>(input, category);
            category = await _blogCategoryRepository.UpdateAsync(category);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(category.Id);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var category = await _blogCategoryRepository.GetAsync(id);

            // 检查是否有子分类
            var childCategories = await GetChildCategoriesAsync(id, false);
            if (childCategories.Any())
            {
                throw new UserFriendlyException("Cannot delete category that has child categories.");
            }

            // 检查是否有关联的文章
            var postCount = await _blogPostRepository.GetCountAsync(categoryId: id);
            if (postCount > 0)
            {
                throw new UserFriendlyException("Cannot delete category that has associated posts.");
            }

            await _blogCategoryRepository.DeleteAsync(category);
        }

        public virtual async Task<BlogCategoryDto> MoveAsync(Guid id, Guid? newParentId)
        {
            var category = await _blogCategoryRepository.GetAsync(id);

            // 如果有新父分类，验证父分类是否存在且不会造成循环引用
            if (newParentId.HasValue)
            {
                if (newParentId.Value == id)
                {
                    throw new UserFriendlyException("Category cannot be its own parent.");
                }

                var parentCategory = await _blogCategoryRepository.FindAsync(newParentId.Value);
                if (parentCategory == null)
                {
                    throw new EntityNotFoundException(typeof(BlogCategory), newParentId.Value);
                }

                if (await WouldCreateCycleAsync(id, newParentId.Value))
                {
                    throw new UserFriendlyException("Cannot move category as it would create a circular reference.");
                }
            }

            category.ParentId = newParentId;
            category = await _blogCategoryRepository.UpdateAsync(category);

            return await GetAsync(category.Id);
        }

        public virtual async Task<BlogCategoryDto> ActivateAsync(Guid id)
        {
            var category = await _blogCategoryRepository.GetAsync(id);
            category.IsActive = true;
            category = await _blogCategoryRepository.UpdateAsync(category);

            return await GetAsync(category.Id);
        }

        public virtual async Task<BlogCategoryDto> DeactivateAsync(Guid id)
        {
            var category = await _blogCategoryRepository.GetAsync(id);
            category.IsActive = false;
            category = await _blogCategoryRepository.UpdateAsync(category);

            return await GetAsync(category.Id);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCategoryBriefDto>> GetCategoryPathAsync(Guid categoryId)
        {
            var path = new List<BlogCategoryBriefDto>();
            var currentId = (Guid?)categoryId;

            while (currentId.HasValue)
            {
                var category = await _blogCategoryRepository.FindAsync(currentId.Value);
                if (category == null) break;

                path.Insert(0, ObjectMapper.Map<BlogCategory, BlogCategoryBriefDto>(category));
                currentId = category.ParentId;
            }

            return path;
        }

        public virtual async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
        {
            return !await _blogCategoryRepository.SlugExistsAsync(slug, excludeId);
        }

        public virtual async Task<string> GenerateSlugAsync(string name)
        {
            var baseSlug = name.ToLower().Replace(" ", "-");
            baseSlug = Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");
            baseSlug = Regex.Replace(baseSlug, @"-+", "-");
            baseSlug = baseSlug.Trim('-');

            var slug = baseSlug;
            var counter = 1;

            while (await _blogCategoryRepository.SlugExistsAsync(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public virtual async Task<BlogCategoryStatisticsDto> GetStatisticsAsync()
        {
            var totalCount = await _blogCategoryRepository.GetCountAsync();
            var categories = await _blogCategoryRepository.GetListAsync();
            var activeCount = categories.Count(c => c.IsActive);
            var rootCount = categories.Count(c => c.ParentId == null);

            return new BlogCategoryStatisticsDto
            {
                TotalCount = (int)totalCount,
                ActiveCount = activeCount,
                InactiveCount = (int)(totalCount - activeCount),
                RootCategoryCount = rootCount
            };
        }

        private List<BlogCategoryTreeDto> BuildCategoryTree(List<BlogCategoryDto> categories)
        {
            var categoryMap = categories.ToDictionary(c => c.Id, c => new BlogCategoryTreeDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ParentId = c.ParentId,
                PostCount = c.PostCount,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                Icon = c.Icon,
                Color = c.Color,
                Children = new List<BlogCategoryTreeDto>()
            });

            var rootCategories = new List<BlogCategoryTreeDto>();

            foreach (var category in categoryMap.Values)
            {
                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
                else if (categoryMap.TryGetValue(category.ParentId.Value, out var parent))
                {
                    parent.Children.Add(category);
                }
            }

            return rootCategories.OrderBy(c => c.Name).ToList();
        }

        private async Task<bool> WouldCreateCycleAsync(Guid categoryId, Guid parentId)
        {
            var currentParentId = (Guid?)parentId;
            while (currentParentId.HasValue)
            {
                if (currentParentId == categoryId)
                {
                    return true;
                }

                var parentCategory = await _blogCategoryRepository.FindAsync(currentParentId.Value);
                currentParentId = parentCategory?.ParentId;
            }

            return false;
        }
    }
}
