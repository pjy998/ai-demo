using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客分类应用服务接口
    /// </summary>
    public interface IBlogCategoryAppService : IApplicationService
    {
        /// <summary>
        /// 获取博客分类列表
        /// </summary>
        Task<PagedResultDto<BlogCategoryDto>> GetListAsync(GetBlogCategoryListDto input);

        /// <summary>
        /// 获取所有活跃分类
        /// </summary>
        Task<List<BlogCategoryBriefDto>> GetActiveCategoriesAsync();

        /// <summary>
        /// 获取分类树
        /// </summary>
        Task<List<BlogCategoryTreeDto>> GetTreeAsync(bool onlyActive = true);

        /// <summary>
        /// 获取根分类
        /// </summary>
        Task<List<BlogCategoryBriefDto>> GetRootCategoriesAsync(bool onlyActive = true);

        /// <summary>
        /// 获取子分类
        /// </summary>
        Task<List<BlogCategoryBriefDto>> GetChildCategoriesAsync(Guid parentId, bool onlyActive = true);

        /// <summary>
        /// 根据ID获取博客分类
        /// </summary>
        Task<BlogCategoryDto> GetAsync(Guid id);

        /// <summary>
        /// 根据Slug获取博客分类
        /// </summary>
        Task<BlogCategoryDto> GetBySlugAsync(string slug);

        /// <summary>
        /// 创建博客分类
        /// </summary>
        Task<BlogCategoryDto> CreateAsync(CreateBlogCategoryDto input);

        /// <summary>
        /// 更新博客分类
        /// </summary>
        Task<BlogCategoryDto> UpdateAsync(Guid id, UpdateBlogCategoryDto input);

        /// <summary>
        /// 删除博客分类
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 移动分类
        /// </summary>
        Task<BlogCategoryDto> MoveAsync(Guid id, Guid? newParentId);

        /// <summary>
        /// 启用分类
        /// </summary>
        Task<BlogCategoryDto> ActivateAsync(Guid id);

        /// <summary>
        /// 禁用分类
        /// </summary>
        Task<BlogCategoryDto> DeactivateAsync(Guid id);

        /// <summary>
        /// 获取分类路径
        /// </summary>
        Task<List<BlogCategoryBriefDto>> GetCategoryPathAsync(Guid categoryId);

        /// <summary>
        /// 检查Slug是否可用
        /// </summary>
        Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null);

        /// <summary>
        /// 生成Slug建议
        /// </summary>
        Task<string> GenerateSlugAsync(string name);

        /// <summary>
        /// 获取分类统计信息
        /// </summary>
        Task<BlogCategoryStatisticsDto> GetStatisticsAsync();
    }

    /// <summary>
    /// 博客分类统计DTO
    /// </summary>
    public class BlogCategoryStatisticsDto
    {
        public int TotalCount { get; set; }
        
        public int ActiveCount { get; set; }
        
        public int InactiveCount { get; set; }
        
        public int RootCategoryCount { get; set; }
        
        public List<BlogCategoryBriefDto> CategoriesWithMostPosts { get; set; } = new List<BlogCategoryBriefDto>();
    }
}
