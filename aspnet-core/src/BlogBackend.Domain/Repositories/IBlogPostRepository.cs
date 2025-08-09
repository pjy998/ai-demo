using BlogBackend.Entities;
using BlogBackend.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BlogBackend.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost, Guid>
    {
        /// <summary>
        /// 根据Slug获取博客文章
        /// </summary>
        Task<BlogPost?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查Slug是否已存在
        /// </summary>
        Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分页的博客文章列表
        /// </summary>
        Task<List<BlogPost>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            BlogPostStatus? status = null,
            Guid? categoryId = null,
            string? keyword = null,
            string sorting = "creationTime desc",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取博客文章总数
        /// </summary>
        Task<long> GetCountAsync(
            BlogPostStatus? status = null,
            Guid? categoryId = null,
            string? keyword = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取已发布的博客文章列表
        /// </summary>
        Task<List<BlogPost>> GetPublishedPostsAsync(
            int skipCount,
            int maxResultCount,
            Guid? categoryId = null,
            List<Guid>? tagIds = null,
            string? keyword = null,
            string sorting = "publishedTime desc",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取已发布博客文章总数
        /// </summary>
        Task<long> GetPublishedCountAsync(
            Guid? categoryId = null,
            List<Guid>? tagIds = null,
            string? keyword = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取相关文章
        /// </summary>
        Task<List<BlogPost>> GetRelatedPostsAsync(
            Guid blogPostId,
            int maxResultCount = 5,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取热门文章
        /// </summary>
        Task<List<BlogPost>> GetPopularPostsAsync(
            int maxResultCount = 10,
            int? dayCount = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取最新文章
        /// </summary>
        Task<List<BlogPost>> GetLatestPostsAsync(
            int maxResultCount = 10,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取博客文章包含标签
        /// </summary>
        Task<BlogPost?> GetWithTagsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量增加浏览次数
        /// </summary>
        Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量增加点赞数
        /// </summary>
        Task IncrementLikeCountAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分类下的博客文章数量
        /// </summary>
        Task<Dictionary<Guid, int>> GetPostCountByCategoryAsync(
            List<Guid> categoryIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取标签下的博客文章数量
        /// </summary>
        Task<Dictionary<Guid, int>> GetPostCountByTagAsync(
            List<Guid> tagIds,
            CancellationToken cancellationToken = default);
    }
}
