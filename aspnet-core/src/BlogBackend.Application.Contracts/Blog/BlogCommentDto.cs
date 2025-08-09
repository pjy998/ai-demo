using BlogBackend.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客评论DTO
    /// </summary>
    public class BlogCommentDto : FullAuditedEntityDto<Guid>
    {
        public Guid BlogPostId { get; set; }
        
        public Guid? ParentCommentId { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string AuthorName { get; set; } = string.Empty;
        
        public string AuthorEmail { get; set; } = string.Empty;
        
        public string? AuthorWebsite { get; set; }
        
        public string? IpAddress { get; set; }
        
        public string? UserAgent { get; set; }
        
        public BlogCommentStatus Status { get; set; }
        
        // 导航属性
        public BlogPostBriefDto? BlogPost { get; set; }
        
        public BlogCommentDto? ParentComment { get; set; }
        
        public List<BlogCommentDto> Replies { get; set; } = new List<BlogCommentDto>();
    }

    /// <summary>
    /// 博客评论简化DTO
    /// </summary>
    public class BlogCommentBriefDto : EntityDto<Guid>
    {
        public Guid BlogPostId { get; set; }
        
        public Guid? ParentCommentId { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string AuthorName { get; set; } = string.Empty;
        
        public string? AuthorWebsite { get; set; }
        
        public BlogCommentStatus Status { get; set; }
        
        public DateTime CreationTime { get; set; }
        
        public string? PostTitle { get; set; }
        
        public int ReplyCount { get; set; }
    }

    /// <summary>
    /// 创建博客评论DTO
    /// </summary>
    public class CreateBlogCommentDto
    {
        public Guid BlogPostId { get; set; }
        
        public Guid? ParentCommentId { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        public string AuthorName { get; set; } = string.Empty;
        
        public string AuthorEmail { get; set; } = string.Empty;
        
        public string? AuthorWebsite { get; set; }
    }

    /// <summary>
    /// 更新博客评论DTO
    /// </summary>
    public class UpdateBlogCommentDto
    {
        public string Content { get; set; } = string.Empty;
        
        public string AuthorName { get; set; } = string.Empty;
        
        public string AuthorEmail { get; set; } = string.Empty;
        
        public string? AuthorWebsite { get; set; }
    }

    /// <summary>
    /// 博客评论查询DTO
    /// </summary>
    public class GetBlogCommentListDto : PagedAndSortedResultRequestDto
    {
        public Guid? BlogPostId { get; set; }
        
        public BlogCommentStatus? Status { get; set; }
        
        public string? Keyword { get; set; }
        
        public string? AuthorEmail { get; set; }
        
        public string? IpAddress { get; set; }
        
        public DateTime? CreatedAfter { get; set; }
        
        public DateTime? CreatedBefore { get; set; }
    }

    /// <summary>
    /// 批量更新评论状态DTO
    /// </summary>
    public class BatchUpdateCommentStatusDto
    {
        public List<Guid> CommentIds { get; set; } = new List<Guid>();
        
        public BlogCommentStatus Status { get; set; }
    }

    /// <summary>
    /// 博客评论统计DTO
    /// </summary>
    public class BlogCommentStatisticsDto
    {
        public int TotalCount { get; set; }
        
        public int PendingCount { get; set; }
        
        public int ApprovedCount { get; set; }
        
        public int RejectedCount { get; set; }
        
        public int SpamCount { get; set; }
        
        public List<BlogCommentBriefDto> LatestComments { get; set; } = new List<BlogCommentBriefDto>();
    }

    /// <summary>
    /// 审核博客评论DTO
    /// </summary>
    public class ModerateBlogCommentDto
    {
        public BlogCommentStatus Status { get; set; }
        
        public string? Reason { get; set; }
    }
}
