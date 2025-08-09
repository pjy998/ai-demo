using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客评论应用服务接口
    /// </summary>
    public interface IBlogCommentAppService : IApplicationService
    {
        /// <summary>
        /// 获取博客评论列表
        /// </summary>
        Task<PagedResultDto<BlogCommentBriefDto>> GetListAsync(GetBlogCommentListDto input);

        /// <summary>
        /// 获取博客文章的评论列表
        /// </summary>
        Task<List<BlogCommentDto>> GetCommentsByPostIdAsync(Guid postId, bool includeReplies = true);

        /// <summary>
        /// 获取评论的回复列表
        /// </summary>
        Task<List<BlogCommentDto>> GetRepliesByCommentIdAsync(Guid parentCommentId);

        /// <summary>
        /// 获取待审核评论
        /// </summary>
        Task<List<BlogCommentBriefDto>> GetPendingCommentsAsync(int maxResultCount = 50);

        /// <summary>
        /// 获取最新评论
        /// </summary>
        Task<List<BlogCommentBriefDto>> GetLatestCommentsAsync(int maxResultCount = 10, bool onlyApproved = true);

        /// <summary>
        /// 根据ID获取博客评论
        /// </summary>
        Task<BlogCommentDto> GetAsync(Guid id);

        /// <summary>
        /// 创建博客评论
        /// </summary>
        Task<BlogCommentDto> CreateAsync(CreateBlogCommentDto input);

        /// <summary>
        /// 更新博客评论
        /// </summary>
        Task<BlogCommentDto> UpdateAsync(Guid id, UpdateBlogCommentDto input);

        /// <summary>
        /// 删除博客评论
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 审核博客评论
        /// </summary>
        Task<BlogCommentDto> ModerateAsync(Guid id, ModerateBlogCommentDto input);

        /// <summary>
        /// 批量审核评论
        /// </summary>
        Task BatchModerateAsync(BatchUpdateCommentStatusDto input);

        /// <summary>
        /// 批量删除评论
        /// </summary>
        Task BatchDeleteAsync(List<Guid> commentIds);

        /// <summary>
        /// 根据邮箱获取评论
        /// </summary>
        Task<List<BlogCommentBriefDto>> GetCommentsByEmailAsync(string email);

        /// <summary>
        /// 根据IP地址获取评论
        /// </summary>
        Task<List<BlogCommentBriefDto>> GetCommentsByIpAddressAsync(string ipAddress);

        /// <summary>
        /// 检查评论是否为垃圾评论
        /// </summary>
        Task<bool> IsSpamAsync(CreateBlogCommentDto input);

        /// <summary>
        /// 获取评论统计信息
        /// </summary>
        Task<BlogCommentStatisticsDto> GetStatisticsAsync();

        /// <summary>
        /// 获取博客文章的评论数
        /// </summary>
        Task<long> GetCommentCountByPostIdAsync(Guid postId);
    }
}
