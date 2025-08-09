using AutoMapper;
using BlogBackend.Blog;
using BlogBackend.Entities;
using BlogBackend.Enums;
using BlogBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客文章应用服务实现
    /// </summary>
    [Authorize]
    public class BlogPostAppService : ApplicationService, IBlogPostAppService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogCategoryRepository _blogCategoryRepository;
        private readonly IBlogTagRepository _blogTagRepository;
        private readonly IBlogCommentRepository _blogCommentRepository;

        public BlogPostAppService(
            IBlogPostRepository blogPostRepository,
            IBlogCategoryRepository blogCategoryRepository,
            IBlogTagRepository blogTagRepository,
            IBlogCommentRepository blogCommentRepository)
        {
            _blogPostRepository = blogPostRepository;
            _blogCategoryRepository = blogCategoryRepository;
            _blogTagRepository = blogTagRepository;
            _blogCommentRepository = blogCommentRepository;
        }

        public virtual async Task<PagedResultDto<BlogPostBriefDto>> GetListAsync(GetBlogPostListDto input)
        {
            var totalCount = await _blogPostRepository.GetCountAsync(
                status: input.Status,
                categoryId: input.CategoryId,
                keyword: input.Keyword);

            var posts = await _blogPostRepository.GetPagedListAsync(
                skipCount: input.SkipCount,
                maxResultCount: input.MaxResultCount,
                status: input.Status,
                categoryId: input.CategoryId,
                keyword: input.Keyword,
                sorting: input.Sorting ?? "CreationTime desc");

            var postDtos = new List<BlogPostBriefDto>();
            foreach (var post in posts)
            {
                var dto = ObjectMapper.Map<BlogPost, BlogPostBriefDto>(post);
                dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
                postDtos.Add(dto);
            }

            return new PagedResultDto<BlogPostBriefDto>(totalCount, postDtos);
        }

        [AllowAnonymous]
        public virtual async Task<PagedResultDto<BlogPostBriefDto>> GetPublishedListAsync(GetBlogPostListDto input)
        {
            var totalCount = await _blogPostRepository.GetPublishedCountAsync(
                categoryId: input.CategoryId,
                tagIds: input.TagIds,
                keyword: input.Keyword);

            var posts = await _blogPostRepository.GetPublishedPostsAsync(
                skipCount: input.SkipCount,
                maxResultCount: input.MaxResultCount,
                categoryId: input.CategoryId,
                tagIds: input.TagIds,
                keyword: input.Keyword,
                sorting: input.Sorting ?? "PublishedTime desc");

            var postDtos = new List<BlogPostBriefDto>();
            foreach (var post in posts)
            {
                var dto = ObjectMapper.Map<BlogPost, BlogPostBriefDto>(post);
                dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
                postDtos.Add(dto);
            }

            return new PagedResultDto<BlogPostBriefDto>(totalCount, postDtos);
        }

        public virtual async Task<BlogPostDto> GetAsync(Guid id)
        {
            var post = await _blogPostRepository.GetWithTagsAsync(id);
            if (post == null)
            {
                throw new EntityNotFoundException(typeof(BlogPost), id);
            }

            var dto = ObjectMapper.Map<BlogPost, BlogPostDto>(post);
            dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(id, BlogCommentStatus.Approved);
            return dto;
        }

        [AllowAnonymous]
        public virtual async Task<BlogPostDto> GetBySlugAsync(string slug)
        {
            var post = await _blogPostRepository.FindBySlugAsync(slug);
            if (post == null)
            {
                throw new EntityNotFoundException(typeof(BlogPost), slug);
            }

            var dto = ObjectMapper.Map<BlogPost, BlogPostDto>(post);
            dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
            return dto;
        }

        public virtual async Task<BlogPostDto> CreateAsync(CreateBlogPostDto input)
        {
            // 检查Slug是否已存在
            if (await _blogPostRepository.SlugExistsAsync(input.Slug))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            var post = ObjectMapper.Map<CreateBlogPostDto, BlogPost>(input);
            post = await _blogPostRepository.InsertAsync(post);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(post.Id);
        }

        public virtual async Task<BlogPostDto> UpdateAsync(Guid id, UpdateBlogPostDto input)
        {
            var post = await _blogPostRepository.GetAsync(id);

            // 检查Slug是否已存在（排除当前文章）
            if (post.Slug != input.Slug && await _blogPostRepository.SlugExistsAsync(input.Slug, id))
            {
                throw new UserFriendlyException($"Slug '{input.Slug}' already exists.");
            }

            ObjectMapper.Map<UpdateBlogPostDto, BlogPost>(input, post);
            post = await _blogPostRepository.UpdateAsync(post);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return await GetAsync(post.Id);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var post = await _blogPostRepository.GetAsync(id);
            await _blogPostRepository.DeleteAsync(post);
        }

        public virtual async Task<BlogPostDto> PublishAsync(Guid id, PublishBlogPostDto input)
        {
            var post = await _blogPostRepository.GetAsync(id);
            post.Status = BlogPostStatus.Published;
            post.PublishedTime = input.PublishedTime ?? DateTime.Now;
            post = await _blogPostRepository.UpdateAsync(post);
            
            return await GetAsync(post.Id);
        }

        public virtual async Task<BlogPostDto> WithdrawAsync(Guid id)
        {
            var post = await _blogPostRepository.GetAsync(id);
            post.Status = BlogPostStatus.Draft;
            post.PublishedTime = null;
            post = await _blogPostRepository.UpdateAsync(post);
            
            return await GetAsync(post.Id);
        }

        [AllowAnonymous]
        public virtual async Task IncrementViewCountAsync(Guid id)
        {
            await _blogPostRepository.IncrementViewCountAsync(id);
        }

        [AllowAnonymous]
        public virtual async Task IncrementLikeCountAsync(Guid id)
        {
            await _blogPostRepository.IncrementLikeCountAsync(id);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogPostBriefDto>> GetRelatedPostsAsync(Guid id, int maxResultCount = 5)
        {
            var posts = await _blogPostRepository.GetRelatedPostsAsync(id, maxResultCount);
            var postDtos = new List<BlogPostBriefDto>();
            
            foreach (var post in posts)
            {
                var dto = ObjectMapper.Map<BlogPost, BlogPostBriefDto>(post);
                dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogPostBriefDto>> GetPopularPostsAsync(int maxResultCount = 10, int? dayCount = null)
        {
            var posts = await _blogPostRepository.GetPopularPostsAsync(maxResultCount, dayCount);
            var postDtos = new List<BlogPostBriefDto>();
            
            foreach (var post in posts)
            {
                var dto = ObjectMapper.Map<BlogPost, BlogPostBriefDto>(post);
                dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogPostBriefDto>> GetLatestPostsAsync(int maxResultCount = 10)
        {
            var posts = await _blogPostRepository.GetLatestPostsAsync(maxResultCount);
            var postDtos = new List<BlogPostBriefDto>();
            
            foreach (var post in posts)
            {
                var dto = ObjectMapper.Map<BlogPost, BlogPostBriefDto>(post);
                dto.CommentCount = (int)await _blogCommentRepository.GetCommentCountByPostIdAsync(post.Id, BlogCommentStatus.Approved);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        public virtual async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
        {
            return !await _blogPostRepository.SlugExistsAsync(slug, excludeId);
        }

        public virtual async Task<string> GenerateSlugAsync(string title)
        {
            var baseSlug = title.ToLower().Replace(" ", "-");
            baseSlug = Regex.Replace(baseSlug, @"[^a-z0-9\-]", "");
            baseSlug = Regex.Replace(baseSlug, @"-+", "-");
            baseSlug = baseSlug.Trim('-');

            var slug = baseSlug;
            var counter = 1;

            while (await _blogPostRepository.SlugExistsAsync(slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public virtual async Task<BlogPostStatisticsDto> GetStatisticsAsync()
        {
            var totalCount = await _blogPostRepository.GetCountAsync();
            var draftCount = await _blogPostRepository.GetCountAsync(status: BlogPostStatus.Draft);
            var publishedCount = await _blogPostRepository.GetCountAsync(status: BlogPostStatus.Published);

            return new BlogPostStatisticsDto
            {
                TotalCount = (int)totalCount,
                DraftCount = (int)draftCount,
                PublishedCount = (int)publishedCount
            };
        }
    }
}
