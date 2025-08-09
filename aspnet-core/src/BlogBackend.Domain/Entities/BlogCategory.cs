using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BlogBackend.Entities
{
    /// <summary>
    /// 博客分类实体
    /// </summary>
    public class BlogCategory : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 分类描述
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// URL友好的分类标识符
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// 父分类ID（支持层级分类）
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父分类导航属性
        /// </summary>
        public virtual BlogCategory? Parent { get; set; }

        /// <summary>
        /// 子分类集合
        /// </summary>
        public virtual ICollection<BlogCategory> Children { get; set; } = new HashSet<BlogCategory>();

        /// <summary>
        /// 该分类下的文章集合
        /// </summary>
        public virtual ICollection<BlogPost> Posts { get; set; } = new HashSet<BlogPost>();

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 分类图标
        /// </summary>
        [MaxLength(100)]
        public string? Icon { get; set; }

        /// <summary>
        /// 分类颜色
        /// </summary>
        [MaxLength(20)]
        public string? Color { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; } = true;

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

        protected BlogCategory()
        {
            // EF Constructor
        }

        public BlogCategory(
            Guid id,
            string name,
            string slug,
            Guid? parentId = null) : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), 100);
            Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), 100);
            ParentId = parentId;
        }

        /// <summary>
        /// 更新基本信息
        /// </summary>
        public void UpdateBasicInfo(string name, string slug, string? description = null)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), 100);
            Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), 100);
            Description = description?.Trim();
        }

        /// <summary>
        /// 设置父分类
        /// </summary>
        public void SetParent(Guid? parentId)
        {
            // 防止循环引用
            if (parentId.HasValue && parentId.Value == Id)
            {
                throw new ArgumentException("分类不能设置自己为父分类");
            }
            
            ParentId = parentId;
        }

        /// <summary>
        /// 设置显示样式
        /// </summary>
        public void SetDisplayStyle(string? icon, string? color)
        {
            Icon = icon?.Trim();
            Color = color?.Trim();
        }

        /// <summary>
        /// 启用分类
        /// </summary>
        public void Enable()
        {
            IsActive = true;
        }

        /// <summary>
        /// 禁用分类
        /// </summary>
        public void Disable()
        {
            IsActive = false;
        }

        /// <summary>
        /// 更新SEO信息
        /// </summary>
        public void UpdateSeoInfo(string? metaKeywords, string? metaDescription)
        {
            MetaKeywords = metaKeywords?.Trim();
            MetaDescription = metaDescription?.Trim();
        }

        /// <summary>
        /// 获取完整路径（包含父分类）
        /// </summary>
        public string GetFullPath()
        {
            var path = Name;
            var current = Parent;
            
            while (current != null)
            {
                path = $"{current.Name} > {path}";
                current = current.Parent;
            }
            
            return path;
        }
    }
}
