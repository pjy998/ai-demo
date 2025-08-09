using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogBackend.Blog;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[RemoteService(Name = "Default")]
[Area("app")]
[Route("api/blog-posts")]
public class BlogPostController : BlogBackendController, IBlogPostAppService
{
    protected IBlogPostAppService BlogPostAppService { get; }

    public BlogPostController(IBlogPostAppService blogPostAppService)
    {
        BlogPostAppService = blogPostAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<BlogPostBriefDto>> GetListAsync(GetBlogPostListDto input)
    {
        return BlogPostAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("published")]
    public virtual Task<PagedResultDto<BlogPostBriefDto>> GetPublishedListAsync(GetBlogPostListDto input)
    {
        return BlogPostAppService.GetPublishedListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<BlogPostDto> GetAsync(Guid id)
    {
        return BlogPostAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("by-slug/{slug}")]
    public virtual Task<BlogPostDto> GetBySlugAsync(string slug)
    {
        return BlogPostAppService.GetBySlugAsync(slug);
    }

    [HttpPost]
    public virtual Task<BlogPostDto> CreateAsync(CreateBlogPostDto input)
    {
        return BlogPostAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<BlogPostDto> UpdateAsync(Guid id, UpdateBlogPostDto input)
    {
        return BlogPostAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return BlogPostAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/publish")]
    public virtual Task<BlogPostDto> PublishAsync(Guid id, PublishBlogPostDto input)
    {
        return BlogPostAppService.PublishAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/withdraw")]
    public virtual Task<BlogPostDto> WithdrawAsync(Guid id)
    {
        return BlogPostAppService.WithdrawAsync(id);
    }

    [HttpPost]
    [Route("{id}/increment-view")]
    public virtual Task IncrementViewCountAsync(Guid id)
    {
        return BlogPostAppService.IncrementViewCountAsync(id);
    }

    [HttpPost]
    [Route("{id}/increment-like")]
    public virtual Task IncrementLikeCountAsync(Guid id)
    {
        return BlogPostAppService.IncrementLikeCountAsync(id);
    }

    [HttpGet]
    [Route("{id}/related")]
    public virtual Task<List<BlogPostBriefDto>> GetRelatedPostsAsync(Guid id, int maxResultCount = 5)
    {
        return BlogPostAppService.GetRelatedPostsAsync(id, maxResultCount);
    }

    [HttpGet]
    [Route("popular")]
    public virtual Task<List<BlogPostBriefDto>> GetPopularPostsAsync(int maxResultCount = 10, int? dayCount = null)
    {
        return BlogPostAppService.GetPopularPostsAsync(maxResultCount, dayCount);
    }

    [HttpGet]
    [Route("latest")]
    public virtual Task<List<BlogPostBriefDto>> GetLatestPostsAsync(int maxResultCount = 10)
    {
        return BlogPostAppService.GetLatestPostsAsync(maxResultCount);
    }

    [HttpGet]
    [Route("slug-available")]
    public virtual Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
    {
        return BlogPostAppService.IsSlugAvailableAsync(slug, excludeId);
    }

    [HttpGet]
    [Route("generate-slug")]
    public virtual Task<string> GenerateSlugAsync(string title)
    {
        return BlogPostAppService.GenerateSlugAsync(title);
    }

    [HttpGet]
    [Route("statistics")]
    public virtual Task<BlogPostStatisticsDto> GetStatisticsAsync()
    {
        return BlogPostAppService.GetStatisticsAsync();
    }
}
