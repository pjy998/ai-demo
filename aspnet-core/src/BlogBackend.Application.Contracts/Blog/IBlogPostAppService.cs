using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客文章应用服务接口
    /// </summary>
    public interface IBlogPostAppService : IApplicationService
    {
        /// <summary>
        /// 获取博客文章列表
        /// </summary>
        Task<PagedResultDto<BlogPostBriefDto>> GetListAsync(GetBlogPostListDto input);

        /// <summary>
        /// 获取已发布的博客文章列表
        /// </summary>
        Task<PagedResultDto<BlogPostBriefDto>> GetPublishedListAsync(GetBlogPostListDto input);

        /// <summary>
        /// 根据ID获取博客文章
        /// </summary>
        Task<BlogPostDto> GetAsync(Guid id);

        /// <summary>
        /// 根据Slug获取博客文章
        /// </summary>
        Task<BlogPostDto> GetBySlugAsync(string slug);

        /// <summary>
        /// 创建博客文章
        /// </summary>
        Task<BlogPostDto> CreateAsync(CreateBlogPostDto input);

        /// <summary>
        /// 更新博客文章
        /// </summary>
        Task<BlogPostDto> UpdateAsync(Guid id, UpdateBlogPostDto input);

        /// <summary>
        /// 删除博客文章
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 发布博客文章
        /// </summary>
        Task<BlogPostDto> PublishAsync(Guid id, PublishBlogPostDto input);

        /// <summary>
        /// 撤回博客文章
        /// </summary>
        Task<BlogPostDto> WithdrawAsync(Guid id);

        /// <summary>
        /// 增加浏览次数
        /// </summary>
        Task IncrementViewCountAsync(Guid id);

        /// <summary>
        /// 增加点赞数
        /// </summary>
        Task IncrementLikeCountAsync(Guid id);

        /// <summary>
        /// 获取相关文章
        /// </summary>
        Task<List<BlogPostBriefDto>> GetRelatedPostsAsync(Guid id, int maxResultCount = 5);

        /// <summary>
        /// 获取热门文章
        /// </summary>
        Task<List<BlogPostBriefDto>> GetPopularPostsAsync(int maxResultCount = 10, int? dayCount = null);

        /// <summary>
        /// 获取最新文章
        /// </summary>
        Task<List<BlogPostBriefDto>> GetLatestPostsAsync(int maxResultCount = 10);

        /// <summary>
        /// 检查Slug是否可用
        /// </summary>
        Task<bool> IsSlugAvailableAsync(string slug, Guid? excludeId = null);

        /// <summary>
        /// 生成Slug建议
        /// </summary>
        Task<string> GenerateSlugAsync(string title);

        /// <summary>
        /// 获取博客文章统计信息
        /// </summary>
        Task<BlogPostStatisticsDto> GetStatisticsAsync();
    }

    /// <summary>
    /// 博客文章统计DTO
    /// </summary>
    public class BlogPostStatisticsDto
    {
        public int TotalCount { get; set; }
        
        public int DraftCount { get; set; }
        
        public int PublishedCount { get; set; }
        
        public int WithdrawnCount { get; set; }
        
        public int TotalViewCount { get; set; }
        
        public int TotalLikeCount { get; set; }
        
        public int TotalCommentCount { get; set; }
    }
}
