using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using BlogBackend.Enums;

namespace BlogBackend.Entities
{
    /// <summary>
    /// 博客文章聚合根实体
    /// </summary>
    public class BlogPost : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 文章简介/摘要
        /// </summary>
        [MaxLength(500)]
        public string? Summary { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// URL友好的文章标识符
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// 文章状态
        /// </summary>
        public BlogPostStatus Status { get; set; } = BlogPostStatus.Draft;

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishedTime { get; set; }

        /// <summary>
        /// 文章分类ID
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// 文章分类导航属性
        /// </summary>
        public virtual BlogCategory? Category { get; set; }

        /// <summary>
        /// 文章标签关系
        /// </summary>
        public virtual ICollection<BlogPostTag> Tags { get; set; } = new HashSet<BlogPostTag>();

        /// <summary>
        /// 文章评论
        /// </summary>
        public virtual ICollection<BlogComment> Comments { get; set; } = new HashSet<BlogComment>();

        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// 点赞次数
        /// </summary>
        public int LikeCount { get; set; } = 0;

        /// <summary>
        /// 封面图片URL
        /// </summary>
        [MaxLength(500)]
        public string? CoverImageUrl { get; set; }

        /// <summary>
        /// SEO关键词
        /// </summary>
        [MaxLength(200)]
        public string? MetaKeywords { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [MaxLength(300)]
        public string? MetaDescription { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComments { get; set; } = true;

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsSticky { get; set; } = false;

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;

        protected BlogPost()
        {
            // EF Constructor
        }

        public BlogPost(
            Guid id,
            string title,
            string content,
            string slug,
            Guid? categoryId = null,
            BlogPostStatus status = BlogPostStatus.Draft) : base(id)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), 200);
            Content = Check.NotNullOrWhiteSpace(content, nameof(content));
            Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), 200);
            CategoryId = categoryId;
            Status = status;
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        public void Publish()
        {
            if (Status == BlogPostStatus.Published)
            {
                return;
            }

            Status = BlogPostStatus.Published;
            PublishedTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 撤回发布
        /// </summary>
        public void Unpublish()
        {
            Status = BlogPostStatus.Draft;
            PublishedTime = null;
        }

        /// <summary>
        /// 增加阅读次数
        /// </summary>
        public void IncreaseViewCount()
        {
            ViewCount++;
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
        /// 设置分类
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        public void SetCategory(Guid? categoryId)
        {
            CategoryId = categoryId;
        }

        /// <summary>
        /// 更新基本信息
        /// </summary>
        public void UpdateBasicInfo(string title, string content, string? summary = null)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), 200);
            Content = Check.NotNullOrWhiteSpace(content, nameof(content));
            Summary = summary?.Trim();
        }

        /// <summary>
        /// 更新SEO信息
        /// </summary>
        public void UpdateSeoInfo(string? metaKeywords, string? metaDescription)
        {
            MetaKeywords = metaKeywords?.Trim();
            MetaDescription = metaDescription?.Trim();
        }
    }
}
