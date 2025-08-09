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
[Route("api/blog-comments")]
public class BlogCommentController : BlogBackendController, IBlogCommentAppService
{
    protected IBlogCommentAppService BlogCommentAppService { get; }

    public BlogCommentController(IBlogCommentAppService blogCommentAppService)
    {
        BlogCommentAppService = blogCommentAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<BlogCommentBriefDto>> GetListAsync(GetBlogCommentListDto input)
    {
        return BlogCommentAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("by-post/{postId}")]
    public virtual Task<List<BlogCommentDto>> GetCommentsByPostIdAsync(Guid postId, bool includeReplies = true)
    {
        return BlogCommentAppService.GetCommentsByPostIdAsync(postId, includeReplies);
    }

    [HttpGet]
    [Route("{parentCommentId}/replies")]
    public virtual Task<List<BlogCommentDto>> GetRepliesByCommentIdAsync(Guid parentCommentId)
    {
        return BlogCommentAppService.GetRepliesByCommentIdAsync(parentCommentId);
    }

    [HttpGet]
    [Route("pending")]
    public virtual Task<List<BlogCommentBriefDto>> GetPendingCommentsAsync(int maxResultCount = 50)
    {
        return BlogCommentAppService.GetPendingCommentsAsync(maxResultCount);
    }

    [HttpGet]
    [Route("latest")]
    public virtual Task<List<BlogCommentBriefDto>> GetLatestCommentsAsync(int maxResultCount = 10, bool onlyApproved = true)
    {
        return BlogCommentAppService.GetLatestCommentsAsync(maxResultCount, onlyApproved);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<BlogCommentDto> GetAsync(Guid id)
    {
        return BlogCommentAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<BlogCommentDto> CreateAsync(CreateBlogCommentDto input)
    {
        return BlogCommentAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<BlogCommentDto> UpdateAsync(Guid id, UpdateBlogCommentDto input)
    {
        return BlogCommentAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return BlogCommentAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/moderate")]
    public virtual Task<BlogCommentDto> ModerateAsync(Guid id, ModerateBlogCommentDto input)
    {
        return BlogCommentAppService.ModerateAsync(id, input);
    }

    [HttpPost]
    [Route("batch-moderate")]
    public virtual Task BatchModerateAsync(BatchUpdateCommentStatusDto input)
    {
        return BlogCommentAppService.BatchModerateAsync(input);
    }

    [HttpPost]
    [Route("batch-delete")]
    public virtual Task BatchDeleteAsync(List<Guid> commentIds)
    {
        return BlogCommentAppService.BatchDeleteAsync(commentIds);
    }

    [HttpGet]
    [Route("by-email/{email}")]
    public virtual Task<List<BlogCommentBriefDto>> GetCommentsByEmailAsync(string email)
    {
        return BlogCommentAppService.GetCommentsByEmailAsync(email);
    }

    [HttpGet]
    [Route("by-ip/{ipAddress}")]
    public virtual Task<List<BlogCommentBriefDto>> GetCommentsByIpAddressAsync(string ipAddress)
    {
        return BlogCommentAppService.GetCommentsByIpAddressAsync(ipAddress);
    }

    [HttpPost]
    [Route("check-spam")]
    public virtual Task<bool> IsSpamAsync(CreateBlogCommentDto input)
    {
        return BlogCommentAppService.IsSpamAsync(input);
    }

    [HttpGet]
    [Route("statistics")]
    public virtual Task<BlogCommentStatisticsDto> GetStatisticsAsync()
    {
        return BlogCommentAppService.GetStatisticsAsync();
    }

    [HttpGet]
    [Route("count/by-post/{postId}")]
    public virtual Task<long> GetCommentCountByPostIdAsync(Guid postId)
    {
        return BlogCommentAppService.GetCommentCountByPostIdAsync(postId);
    }
}
