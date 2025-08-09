using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客标签应用服务接口
    /// </summary>
    public interface IBlogTagAppService : IApplicationService
    {
        /// <summary>
        /// 获取博客标签列表
        /// </summary>
        Task<PagedResultDto<BlogTagDto>> GetListAsync(GetBlogTagListDto input);

        /// <summary>
        /// 获取所有活跃标签
        /// </summary>
        Task<List<BlogTagBriefDto>> GetActiveTagsAsync();

        /// <summary>
        /// 获取热门标签
        /// </summary>
        Task<List<BlogTagBriefDto>> GetPopularTagsAsync(int maxResultCount = 20);

        /// <summary>
        /// 根据ID获取博客标签
        /// </summary>
        Task<BlogTagDto> GetAsync(Guid id);

        /// <summary>
        /// 根据Slug获取博客标签
        /// </summary>
        Task<BlogTagDto> GetBySlugAsync(string slug);

        /// <summary>
        /// 根据名称获取博客标签
        /// </summary>
        Task<BlogTagDto> GetByNameAsync(string name);

        /// <summary>
        /// 创建博客标签
        /// </summary>
        Task<BlogTagDto> CreateAsync(CreateBlogTagDto input);

        /// <summary>
        /// 更新博客标签
        /// </summary>
        Task<BlogTagDto> UpdateAsync(Guid id, UpdateBlogTagDto input);

        /// <summary>
        /// 删除博客标签
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 启用标签
        /// </summary>
        Task<BlogTagDto> ActivateAsync(Guid id);

        /// <summary>
        /// 禁用标签
        /// </summary>
        Task<BlogTagDto> DeactivateAsync(Guid id);

        /// <summary>
        /// 根据名称批量获取或创建标签
        /// </summary>
        Task<List<BlogTagDto>> GetOrCreateByNamesAsync(List<string> tagNames);

        /// <summary>
        /// 获取博客文章的标签
        /// </summary>
        Task<List<BlogTagDto>> GetTagsByPostIdAsync(Guid postId);

        /// <summary>
        /// 清理未使用的标签
        /// </summary>
        Task<int> CleanupUnusedTagsAsync();

        /// <summary>
        /// 检查Slug是否可用
        /// </summary>
        Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null);

        /// <summary>
        /// 检查名称是否可用
        /// </summary>
        Task<bool> IsNameAvailableAsync(string name, Guid? excludeId = null);

        /// <summary>
        /// 生成Slug建议
        /// </summary>
        Task<string> GenerateSlugAsync(string name);

        /// <summary>
        /// 获取标签统计信息
        /// </summary>
        Task<BlogTagStatisticsDto> GetStatisticsAsync();
    }
}
