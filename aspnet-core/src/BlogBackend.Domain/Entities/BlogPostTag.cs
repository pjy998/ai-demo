using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace BlogBackend.Entities
{
    /// <summary>
    /// 博客文章与标签的关联实体
    /// </summary>
    public class BlogPostTag : CreationAuditedEntity
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required]
        public Guid BlogPostId { get; set; }

        /// <summary>
        /// 标签ID
        /// </summary>
        [Required]
        public Guid BlogTagId { get; set; }

        /// <summary>
        /// 文章导航属性
        /// </summary>
        public virtual BlogPost BlogPost { get; set; } = null!;

        /// <summary>
        /// 标签导航属性
        /// </summary>
        public virtual BlogTag BlogTag { get; set; } = null!;

        protected BlogPostTag()
        {
            // EF Constructor
        }

        public BlogPostTag(Guid blogPostId, Guid blogTagId)
        {
            BlogPostId = blogPostId;
            BlogTagId = blogTagId;
        }

        /// <summary>
        /// 重写GetKeys方法，使用组合键
        /// </summary>
        public override object[] GetKeys()
        {
            return new object[] { BlogPostId, BlogTagId };
        }
    }
}
