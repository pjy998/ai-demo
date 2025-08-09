using BlogBackend.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BlogBackend.Repositories
{
    public interface IBlogTagRepository : IRepository<BlogTag, Guid>
    {
        /// <summary>
        /// 根据Slug获取标签
        /// </summary>
        Task<BlogTag?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据名称获取标签
        /// </summary>
        Task<BlogTag?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查Slug是否已存在
        /// </summary>
        Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查名称是否已存在
        /// </summary>
        Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取标签列表
        /// </summary>
        Task<List<BlogTag>> GetListAsync(
            bool onlyActive = true,
            string? keyword = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取热门标签
        /// </summary>
        Task<List<BlogTag>> GetPopularTagsAsync(
            int maxResultCount = 20,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据名称批量获取或创建标签
        /// </summary>
        Task<List<BlogTag>> GetOrCreateTagsByNamesAsync(
            List<string> tagNames,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取博客文章的标签
        /// </summary>
        Task<List<BlogTag>> GetTagsByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新标签使用次数
        /// </summary>
        Task UpdateUsageCountAsync(
            Guid tagId,
            int increment = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量更新标签使用次数
        /// </summary>
        Task BatchUpdateUsageCountAsync(
            Dictionary<Guid, int> tagUsageChanges,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取未使用的标签
        /// </summary>
        Task<List<BlogTag>> GetUnusedTagsAsync(
            CancellationToken cancellationToken = default);
    }
}
