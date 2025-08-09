using BlogBackend.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BlogBackend.Repositories
{
    public interface IBlogCategoryRepository : IRepository<BlogCategory, Guid>
    {
        /// <summary>
        /// 根据Slug获取分类
        /// </summary>
        Task<BlogCategory?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查Slug是否已存在
        /// </summary>
        Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有根分类
        /// </summary>
        Task<List<BlogCategory>> GetRootCategoriesAsync(
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取子分类
        /// </summary>
        Task<List<BlogCategory>> GetChildCategoriesAsync(
            Guid parentId,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分类树
        /// </summary>
        Task<List<BlogCategory>> GetCategoryTreeAsync(
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分类的完整路径
        /// </summary>
        Task<List<BlogCategory>> GetCategoryPathAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有子孙分类ID
        /// </summary>
        Task<List<Guid>> GetDescendantIdsAsync(
            Guid parentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查是否有子分类
        /// </summary>
        Task<bool> HasChildrenAsync(
            Guid parentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查分类是否有博客文章
        /// </summary>
        Task<bool> HasPostsAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取活跃分类列表
        /// </summary>
        Task<List<BlogCategory>> GetActiveCategoriesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查是否会形成循环引用
        /// </summary>
        Task<bool> WillCreateCircularReferenceAsync(
            Guid categoryId,
            Guid parentId,
            CancellationToken cancellationToken = default);
    }
}
