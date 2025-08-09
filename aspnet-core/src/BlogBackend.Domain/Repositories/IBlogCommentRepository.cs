using BlogBackend.Entities;
using BlogBackend.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BlogBackend.Repositories
{
    public interface IBlogCommentRepository : IRepository<BlogComment, Guid>
    {
        /// <summary>
        /// 获取博客文章的评论列表
        /// </summary>
        Task<List<BlogComment>> GetCommentsByPostIdAsync(
            Guid postId,
            BlogCommentStatus? status = null,
            bool includeReplies = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取博客文章的评论数
        /// </summary>
        Task<long> GetCommentCountByPostIdAsync(
            Guid postId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取评论的回复列表
        /// </summary>
        Task<List<BlogComment>> GetRepliesByCommentIdAsync(
            Guid parentCommentId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取评论的回复数
        /// </summary>
        Task<long> GetReplyCountByCommentIdAsync(
            Guid parentCommentId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分页评论列表
        /// </summary>
        Task<List<BlogComment>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            Guid? postId = null,
            BlogCommentStatus? status = null,
            string? keyword = null,
            string sorting = "creationTime desc",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取评论总数
        /// </summary>
        Task<long> GetCountAsync(
            Guid? postId = null,
            BlogCommentStatus? status = null,
            string? keyword = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取待审核评论
        /// </summary>
        Task<List<BlogComment>> GetPendingCommentsAsync(
            int maxResultCount = 50,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取待审核评论数
        /// </summary>
        Task<long> GetPendingCommentCountAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取最新评论
        /// </summary>
        Task<List<BlogComment>> GetLatestCommentsAsync(
            int maxResultCount = 10,
            bool onlyApproved = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据邮箱获取评论
        /// </summary>
        Task<List<BlogComment>> GetCommentsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据IP地址获取评论
        /// </summary>
        Task<List<BlogComment>> GetCommentsByIpAddressAsync(
            string ipAddress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量更新评论状态
        /// </summary>
        Task BatchUpdateStatusAsync(
            List<Guid> commentIds,
            BlogCommentStatus status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除博客文章的所有评论
        /// </summary>
        Task DeleteByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取评论统计信息
        /// </summary>
        Task<Dictionary<BlogCommentStatus, int>> GetCommentStatisticsAsync(
            CancellationToken cancellationToken = default);
    }
}
