using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BlogBackend.Entities
{
    /// <summary>
    /// 博客标签实体
    /// </summary>
    public class BlogTag : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL友好的标签标识符
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// 标签描述
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// 标签颜色
        /// </summary>
        [MaxLength(20)]
        public string? Color { get; set; }

        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsageCount { get; set; } = 0;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 标签与文章的关系
        /// </summary>
        public virtual ICollection<BlogPostTag> Posts { get; set; } = new HashSet<BlogPostTag>();

        protected BlogTag()
        {
            // EF Constructor
        }

        public BlogTag(
            Guid id,
            string name,
            string slug) : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), 50);
            Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), 50);
        }

        /// <summary>
        /// 更新基本信息
        /// </summary>
        public void UpdateBasicInfo(string name, string slug, string? description = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), 50);
            Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), 50);
            Description = description?.Trim();
        }

        /// <summary>
        /// 增加使用次数
        /// </summary>
        public void IncreaseUsageCount()
        {
            UsageCount++;
        }

        /// <summary>
        /// 减少使用次数
        /// </summary>
        public void DecreaseUsageCount()
        {
            if (UsageCount > 0)
            {
                UsageCount--;
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        public void SetColor(string? color)
        {
            Color = color?.Trim();
        }

        /// <summary>
        /// 启用标签
        /// </summary>
        public void Enable()
        {
            IsActive = true;
        }

        /// <summary>
        /// 禁用标签
        /// </summary>
        public void Disable()
        {
            IsActive = false;
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        public static BlogTag Create(string name, string slug, string? description = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));
            
            return new BlogTag
            {
                Name = name,
                Slug = slug,
                Description = description,
                UsageCount = 0,
                IsActive = true
            };
        }
    }
}
