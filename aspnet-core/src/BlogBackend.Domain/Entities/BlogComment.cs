using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using BlogBackend.Enums;

namespace BlogBackend.Entities
{
    /// <summary>
    /// 博客评论实体
    /// </summary>
    public class BlogComment : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 所属文章ID
        /// </summary>
        [Required]
        public Guid BlogPostId { get; set; }

        /// <summary>
        /// 所属文章导航属性
        /// </summary>
        public virtual BlogPost BlogPost { get; set; } = null!;

        /// <summary>
        /// 父评论ID（用于回复功能）
        /// </summary>
        public Guid? ParentCommentId { get; set; }

        /// <summary>
        /// 父评论导航属性
        /// </summary>
        public virtual BlogComment? ParentComment { get; set; }

        /// <summary>
        /// 子评论集合
        /// </summary>
        public virtual ICollection<BlogComment> Replies { get; set; } = new HashSet<BlogComment>();

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 评论者姓名
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// 评论者邮箱
        /// </summary>
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string AuthorEmail { get; set; } = string.Empty;

        /// <summary>
        /// 评论者网站
        /// </summary>
        [MaxLength(200)]
        public string? AuthorWebsite { get; set; }

        /// <summary>
        /// 评论者IP地址
        /// </summary>
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 评论者User Agent
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// 评论状态
        /// </summary>
        public BlogCommentStatus Status { get; set; } = BlogCommentStatus.Pending;

        /// <summary>
        /// 点赞次数
        /// </summary>
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 评论层级深度
        /// </summary>
        public int Depth { get; set; } = 0;

        /// <summary>
        /// 是否来自注册用户
        /// </summary>
        public bool IsFromRegisteredUser { get; set; } = false;

        /// <summary>
        /// 注册用户ID（可选）
        /// </summary>
        public Guid? UserId { get; set; }

        protected BlogComment()
        {
            // EF Constructor
        }

        public BlogComment(
            Guid id,
            Guid blogPostId,
            string content,
            string authorName,
            string authorEmail,
            Guid? parentCommentId = null) : base(id)
        {
            BlogPostId = blogPostId;
            Content = Check.NotNullOrWhiteSpace(content, nameof(content), 2000);
            AuthorName = Check.NotNullOrWhiteSpace(authorName, nameof(authorName), 100);
            AuthorEmail = Check.NotNullOrWhiteSpace(authorEmail, nameof(authorEmail), 200);
            ParentCommentId = parentCommentId;
            
            // 计算层级深度
            Depth = parentCommentId.HasValue ? 1 : 0; // 简化处理，实际应该递归计算
        }

        /// <summary>
        /// 更新评论内容
        /// </summary>
        public void UpdateContent(string content)
        {
            Content = Check.NotNullOrWhiteSpace(content, nameof(content), 2000);
        }

        /// <summary>
        /// 更新作者信息
        /// </summary>
        public void UpdateAuthorInfo(string authorName, string authorEmail, string? authorWebsite = null)
        {
            AuthorName = Check.NotNullOrWhiteSpace(authorName, nameof(authorName), 100);
            AuthorEmail = Check.NotNullOrWhiteSpace(authorEmail, nameof(authorEmail), 200);
            AuthorWebsite = authorWebsite?.Trim();
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Approve()
        {
            Status = BlogCommentStatus.Approved;
        }

        /// <summary>
        /// 拒绝评论
        /// </summary>
        public void Reject()
        {
            Status = BlogCommentStatus.Rejected;
        }

        /// <summary>
        /// 标记为垃圾评论
        /// </summary>
        public void MarkAsSpam()
        {
            Status = BlogCommentStatus.Spam;
        }

        /// <summary>
        /// 增加点赞次数
        /// </summary>
        public void IncreaseLikeCount()
        {
            LikeCount++;
        }

        /// <summary>
        /// 减少点赞次数
        /// </summary>
        public void DecreaseLikeCount()
        {
            if (LikeCount > 0)
            {
                LikeCount--;
            }
        }

        /// <summary>
        /// 设置客户端信息
        /// </summary>
        public void SetClientInfo(string? ipAddress, string? userAgent)
        {
            IpAddress = ipAddress?.Trim();
            UserAgent = userAgent?.Trim();
        }

        /// <summary>
        /// 关联注册用户
        /// </summary>
        public void AssociateWithUser(Guid userId)
        {
            UserId = userId;
            IsFromRegisteredUser = true;
        }

        /// <summary>
        /// 计算回复层级深度
        /// </summary>
        public void CalculateDepth(int parentDepth = 0)
        {
            Depth = parentDepth + 1;
        }
    }
}
