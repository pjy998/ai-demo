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
[Route("api/blog-tags")]
public class BlogTagController : BlogBackendController, IBlogTagAppService
{
    protected IBlogTagAppService BlogTagAppService { get; }

    public BlogTagController(IBlogTagAppService blogTagAppService)
    {
        BlogTagAppService = blogTagAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<BlogTagDto>> GetListAsync(GetBlogTagListDto input)
    {
        return BlogTagAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("active")]
    public virtual Task<List<BlogTagBriefDto>> GetActiveTagsAsync()
    {
        return BlogTagAppService.GetActiveTagsAsync();
    }

    [HttpGet]
    [Route("popular")]
    public virtual Task<List<BlogTagBriefDto>> GetPopularTagsAsync(int maxResultCount = 20)
    {
        return BlogTagAppService.GetPopularTagsAsync(maxResultCount);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<BlogTagDto> GetAsync(Guid id)
    {
        return BlogTagAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("by-slug/{slug}")]
    public virtual Task<BlogTagDto> GetBySlugAsync(string slug)
    {
        return BlogTagAppService.GetBySlugAsync(slug);
    }

    [HttpGet]
    [Route("by-name/{name}")]
    public virtual Task<BlogTagDto> GetByNameAsync(string name)
    {
        return BlogTagAppService.GetByNameAsync(name);
    }

    [HttpPost]
    public virtual Task<BlogTagDto> CreateAsync(CreateBlogTagDto input)
    {
        return BlogTagAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<BlogTagDto> UpdateAsync(Guid id, UpdateBlogTagDto input)
    {
        return BlogTagAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return BlogTagAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task<BlogTagDto> ActivateAsync(Guid id)
    {
        return BlogTagAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task<BlogTagDto> DeactivateAsync(Guid id)
    {
        return BlogTagAppService.DeactivateAsync(id);
    }

    [HttpPost]
    [Route("get-or-create")]
    public virtual Task<List<BlogTagDto>> GetOrCreateByNamesAsync(List<string> tagNames)
    {
        return BlogTagAppService.GetOrCreateByNamesAsync(tagNames);
    }

    [HttpGet]
    [Route("by-post/{postId}")]
    public virtual Task<List<BlogTagDto>> GetTagsByPostIdAsync(Guid postId)
    {
        return BlogTagAppService.GetTagsByPostIdAsync(postId);
    }

    [HttpPost]
    [Route("cleanup-unused")]
    public virtual Task<int> CleanupUnusedTagsAsync()
    {
        return BlogTagAppService.CleanupUnusedTagsAsync();
    }

    [HttpGet]
    [Route("slug-available")]
    public virtual Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
    {
        return BlogTagAppService.IsSlugAvailableAsync(slug, excludeId);
    }

    [HttpGet]
    [Route("name-available")]
    public virtual Task<bool> IsNameAvailableAsync(string name, Guid? excludeId = null)
    {
        return BlogTagAppService.IsNameAvailableAsync(name, excludeId);
    }

    [HttpGet]
    [Route("generate-slug")]
    public virtual Task<string> GenerateSlugAsync(string name)
    {
        return BlogTagAppService.GenerateSlugAsync(name);
    }

    [HttpGet]
    [Route("statistics")]
    public virtual Task<BlogTagStatisticsDto> GetStatisticsAsync()
    {
        return BlogTagAppService.GetStatisticsAsync();
    }
}
