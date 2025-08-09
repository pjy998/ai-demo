using BlogBackend.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客文章DTO
    /// </summary>
    public class BlogPostDto : FullAuditedEntityDto<Guid>
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Summary { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? CoverImageUrl { get; set; }
        
        public BlogPostStatus Status { get; set; }
        
        public DateTime? PublishedTime { get; set; }
        
        public int ViewCount { get; set; }
        
        public int LikeCount { get; set; }
        
        public int CommentCount { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public bool AllowComment { get; set; }
        
        public int SortOrder { get; set; }
        
        // 导航属性
        public BlogCategoryDto? Category { get; set; }
        
        public List<BlogTagDto> Tags { get; set; } = new List<BlogTagDto>();
    }

    /// <summary>
    /// 博客文章简化DTO
    /// </summary>
    public class BlogPostBriefDto : EntityDto<Guid>
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Summary { get; set; }
        
        public string Slug { get; set; } = string.Empty;
        
        public string? CoverImageUrl { get; set; }
        
        public BlogPostStatus Status { get; set; }
        
        public DateTime? PublishedTime { get; set; }
        
        public int ViewCount { get; set; }
        
        public int LikeCount { get; set; }
        
        public int CommentCount { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        
        public List<string> TagNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// 创建博客文章DTO
    /// </summary>
    public class CreateBlogPostDto
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Summary { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? CoverImageUrl { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public bool AllowComment { get; set; } = true;
        
        public int SortOrder { get; set; }
        
        public List<string> TagNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// 更新博客文章DTO
    /// </summary>
    public class UpdateBlogPostDto
    {
        public string Title { get; set; } = string.Empty;
        
        public string? Summary { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? CoverImageUrl { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public bool AllowComment { get; set; }
        
        public int SortOrder { get; set; }
        
        public List<string> TagNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// 博客文章查询DTO
    /// </summary>
    public class GetBlogPostListDto : PagedAndSortedResultRequestDto
    {
        public BlogPostStatus? Status { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public string? Keyword { get; set; }
        
        public List<Guid>? TagIds { get; set; }
        
        public bool? IsFeatured { get; set; }
        
        public DateTime? PublishedAfter { get; set; }
        
        public DateTime? PublishedBefore { get; set; }
    }

    /// <summary>
    /// 发布博客文章DTO
    /// </summary>
    public class PublishBlogPostDto
    {
        public DateTime? PublishedTime { get; set; }
    }
}
