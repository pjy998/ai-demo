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
    /// 博客标签应用服务实现
    /// </summary>
    [Authorize]
    public class BlogTagAppService : ApplicationService, IBlogTagAppService
    {
        private readonly IBlogTagRepository _blogTagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogTagAppService(
            IBlogTagRepository blogTagRepository,
            IBlogPostRepository blogPostRepository)
        {
            _blogTagRepository = blogTagRepository;
            _blogPostRepository = blogPostRepository;
        }

        public virtual async Task<PagedResultDto<BlogTagDto>> GetListAsync(GetBlogTagListDto input)
        {
            var totalCount = await _blogTagRepository.GetCountAsync();
            var tags = await _blogTagRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                input.Sorting ?? "Name");

            var tagDtos = new List<BlogTagDto>();
            foreach (var tag in tags)
            {
                var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(tag);
                // 简化：不统计使用次数，设为0
                dto.UsageCount = 0;
                tagDtos.Add(dto);
            }

            return new PagedResultDto<BlogTagDto>(totalCount, tagDtos);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogTagBriefDto>> GetActiveTagsAsync()
        {
            var tags = await _blogTagRepository.GetListAsync();
            return tags
                .Where(t => t.IsActive)
                .Select(t => ObjectMapper.Map<BlogTag, BlogTagBriefDto>(t))
                .ToList();
        }

        public virtual async Task<BlogTagDto> GetAsync(Guid id)
        {
            var tag = await _blogTagRepository.GetAsync(id);
            var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(tag);
            dto.UsageCount = 0; // 简化
            return dto;
        }

        [AllowAnonymous]
        public virtual async Task<BlogTagDto> GetBySlugAsync(string slug)
        {
            var tag = await _blogTagRepository.FindBySlugAsync(slug);
            if (tag == null)
            {
                throw new EntityNotFoundException(typeof(BlogTag), slug);
            }

            var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(tag);
            dto.UsageCount = 0; // 简化
            return dto;
        }

        [AllowAnonymous]
        public virtual async Task<BlogTagDto> GetByNameAsync(string name)
        {
            var tag = await _blogTagRepository.FindByNameAsync(name);
            if (tag == null)
            {
                throw new EntityNotFoundException(typeof(BlogTag), name);
            }

            var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(tag);
            dto.UsageCount = 0; // 简化
            return dto;
        }

        public virtual async Task<BlogTagDto> CreateAsync(CreateBlogTagDto input)
        {
            // 检查名称是否已存在
            if (await _blogTagRepository.NameExistsAsync(input.Name))
            {
                throw new UserFriendlyException($"Tag name '{input.Name}' already exists.");
            }

            // 检查Slug是否已存在
            if (await _blogTagRepository.SlugExistsAsync(input.Slug))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            var tag = ObjectMapper.Map<CreateBlogTagDto, BlogTag>(input);
            tag = await _blogTagRepository.InsertAsync(tag);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(tag.Id);
        }

        public virtual async Task<BlogTagDto> UpdateAsync(Guid id, UpdateBlogTagDto input)
        {
            var tag = await _blogTagRepository.GetAsync(id);

            // 检查名称是否已存在（排除当前标签）
            if (tag.Name != input.Name && await _blogTagRepository.NameExistsAsync(input.Name, id))
            {
                throw new UserFriendlyException($"Tag name '{input.Name}' already exists.");
            }

            // 检查Slug是否已存在（排除当前标签）
            if (tag.Slug != input.Slug && await _blogTagRepository.SlugExistsAsync(input.Slug, id))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            ObjectMapper.Map<UpdateBlogTagDto, BlogTag>(input, tag);
            tag = await _blogTagRepository.UpdateAsync(tag);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(tag.Id);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var tag = await _blogTagRepository.GetAsync(id);
            await _blogTagRepository.DeleteAsync(tag);
        }

        public virtual async Task<BlogTagDto> ActivateAsync(Guid id)
        {
            var tag = await _blogTagRepository.GetAsync(id);
            tag.IsActive = true;
            tag = await _blogTagRepository.UpdateAsync(tag);

            return await GetAsync(tag.Id);
        }

        public virtual async Task<BlogTagDto> DeactivateAsync(Guid id)
        {
            var tag = await _blogTagRepository.GetAsync(id);
            tag.IsActive = false;
            tag = await _blogTagRepository.UpdateAsync(tag);

            return await GetAsync(tag.Id);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogTagBriefDto>> GetPopularTagsAsync(int maxResultCount = 10)
        {
            var tags = await _blogTagRepository.GetListAsync();
            return tags
                .Where(t => t.IsActive)
                .Take(maxResultCount)
                .Select(t => ObjectMapper.Map<BlogTag, BlogTagBriefDto>(t))
                .ToList();
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogTagBriefDto>> GetTagSuggestionsAsync(string keyword, int maxResultCount = 10)
        {
            var tags = await _blogTagRepository.GetListAsync();
            return tags
                .Where(t => t.IsActive && t.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Take(maxResultCount)
                .Select(t => ObjectMapper.Map<BlogTag, BlogTagBriefDto>(t))
                .ToList();
        }

        public virtual async Task<List<BlogTagDto>> GetOrCreateByNamesAsync(List<string> names)
        {
            var result = new List<BlogTagDto>();
            foreach (var name in names)
            {
                var tag = await _blogTagRepository.FindByNameAsync(name);
                if (tag == null)
                {
                    var createDto = new CreateBlogTagDto
                    {
                        Name = name,
                        Slug = await GenerateSlugAsync(name),
                        IsActive = true
                    };
                    var newTag = ObjectMapper.Map<CreateBlogTagDto, BlogTag>(createDto);
                    tag = await _blogTagRepository.InsertAsync(newTag);
                }
                
                var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(tag);
                dto.UsageCount = 0; // 简化
                result.Add(dto);
            }

            return result;
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogTagDto>> GetTagsByPostIdAsync(Guid postId)
        {
            var tags = await _blogTagRepository.GetTagsByPostIdAsync(postId);
            return tags.Select(t =>
            {
                var dto = ObjectMapper.Map<BlogTag, BlogTagDto>(t);
                dto.UsageCount = 0; // 简化
                return dto;
            }).ToList();
        }

        public virtual async Task<int> CleanupUnusedTagsAsync()
        {
            // 简化实现：返回0表示没有清理任何标签
            return await Task.FromResult(0);
        }

        public virtual async Task<bool> IsNameAvailableAsync(string name, Guid? excludeId = null)
        {
            return !await _blogTagRepository.NameExistsAsync(name, excludeId);
        }

        public virtual async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
        {
            return !await _blogTagRepository.SlugExistsAsync(slug, excludeId);
        }

        public virtual async Task<string> GenerateSlugAsync(string name)
        {
            var baseSlug = name.ToLower().Replace(" ", "-");
            baseSlug = Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");
            baseSlug = Regex.Replace(baseSlug, @"-+", "-");
            baseSlug = baseSlug.Trim('-');

            var slug = baseSlug;
            var counter = 1;

            while (await _blogTagRepository.SlugExistsAsync(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogTagStatisticsDto>> GetTagCloudAsync(int maxResultCount = 50)
        {
            var tags = await _blogTagRepository.GetListAsync();
            return tags
                .Where(t => t.IsActive)
                .Take(maxResultCount)
                .Select(t => new BlogTagStatisticsDto
                {
                    TotalCount = 1,
                    ActiveCount = t.IsActive ? 1 : 0,
                    UnusedCount = 0
                })
                .ToList();
        }

        public virtual async Task<BlogTagStatisticsDto> GetStatisticsAsync()
        {
            var totalCount = await _blogTagRepository.GetCountAsync();
            var tags = await _blogTagRepository.GetListAsync();
            var activeCount = tags.Count(t => t.IsActive);

            return new BlogTagStatisticsDto
            {
                TotalCount = (int)totalCount,
                ActiveCount = activeCount,
                UnusedCount = (int)(totalCount - activeCount)
            };
        }
    }
}
