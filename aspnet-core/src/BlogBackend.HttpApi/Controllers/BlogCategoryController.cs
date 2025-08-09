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
[Route("api/blog-categories")]
public class BlogCategoryController : BlogBackendController, IBlogCategoryAppService
{
    protected IBlogCategoryAppService BlogCategoryAppService { get; }

    public BlogCategoryController(IBlogCategoryAppService blogCategoryAppService)
    {
        BlogCategoryAppService = blogCategoryAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<BlogCategoryDto>> GetListAsync(GetBlogCategoryListDto input)
    {
        return BlogCategoryAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("active")]
    public virtual Task<List<BlogCategoryBriefDto>> GetActiveCategoriesAsync()
    {
        return BlogCategoryAppService.GetActiveCategoriesAsync();
    }

    [HttpGet]
    [Route("tree")]
    public virtual Task<List<BlogCategoryTreeDto>> GetTreeAsync(bool onlyActive = true)
    {
        return BlogCategoryAppService.GetTreeAsync(onlyActive);
    }

    [HttpGet]
    [Route("root")]
    public virtual Task<List<BlogCategoryBriefDto>> GetRootCategoriesAsync(bool onlyActive = true)
    {
        return BlogCategoryAppService.GetRootCategoriesAsync(onlyActive);
    }

    [HttpGet]
    [Route("{parentId}/children")]
    public virtual Task<List<BlogCategoryBriefDto>> GetChildCategoriesAsync(Guid parentId, bool onlyActive = true)
    {
        return BlogCategoryAppService.GetChildCategoriesAsync(parentId, onlyActive);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<BlogCategoryDto> GetAsync(Guid id)
    {
        return BlogCategoryAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("by-slug/{slug}")]
    public virtual Task<BlogCategoryDto> GetBySlugAsync(string slug)
    {
        return BlogCategoryAppService.GetBySlugAsync(slug);
    }

    [HttpPost]
    public virtual Task<BlogCategoryDto> CreateAsync(CreateBlogCategoryDto input)
    {
        return BlogCategoryAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<BlogCategoryDto> UpdateAsync(Guid id, UpdateBlogCategoryDto input)
    {
        return BlogCategoryAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return BlogCategoryAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/move")]
    public virtual Task<BlogCategoryDto> MoveAsync(Guid id, Guid? newParentId)
    {
        return BlogCategoryAppService.MoveAsync(id, newParentId);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task<BlogCategoryDto> ActivateAsync(Guid id)
    {
        return BlogCategoryAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task<BlogCategoryDto> DeactivateAsync(Guid id)
    {
        return BlogCategoryAppService.DeactivateAsync(id);
    }

    [HttpGet]
    [Route("{categoryId}/path")]
    public virtual Task<List<BlogCategoryBriefDto>> GetCategoryPathAsync(Guid categoryId)
    {
        return BlogCategoryAppService.GetCategoryPathAsync(categoryId);
    }

    [HttpGet]
    [Route("slug-available")]
    public virtual Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null)
    {
        return BlogCategoryAppService.IsSlugAvailableAsync(slug, excludeId);
    }

    [HttpGet]
    [Route("generate-slug")]
    public virtual Task<string> GenerateSlugAsync(string name)
    {
        return BlogCategoryAppService.GenerateSlugAsync(name);
    }

    [HttpGet]
    [Route("statistics")]
    public virtual Task<BlogCategoryStatisticsDto> GetStatisticsAsync()
    {
        return BlogCategoryAppService.GetStatisticsAsync();
    }
}
