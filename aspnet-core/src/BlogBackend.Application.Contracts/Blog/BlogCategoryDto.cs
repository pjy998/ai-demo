using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客分类DTO
    /// </summary>
    public class BlogCategoryDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Slug { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        public int SortOrder { get; set; }
        
        public string? Icon { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public int PostCount { get; set; }
        
        // 导航属性
        public BlogCategoryDto? Parent { get; set; }
        
        public List<BlogCategoryDto> Children { get; set; } = new List<BlogCategoryDto>();
    }

    /// <summary>
    /// 博客分类简化DTO
    /// </summary>
    public class BlogCategoryBriefDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        public string? Icon { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; }
        
        public int PostCount { get; set; }
    }

    /// <summary>
    /// 创建博客分类DTO
    /// </summary>
    public class CreateBlogCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Slug { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        public int SortOrder { get; set; }
        
        public string? Icon { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
    }

    /// <summary>
    /// 更新博客分类DTO
    /// </summary>
    public class UpdateBlogCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string Slug { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        public int SortOrder { get; set; }
        
        public string? Icon { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public string? MetaDescription { get; set; }
    }

    /// <summary>
    /// 博客分类查询DTO
    /// </summary>
    public class GetBlogCategoryListDto : PagedAndSortedResultRequestDto
    {
        public Guid? ParentId { get; set; }
        
        public bool? IsActive { get; set; }
        
        public string? Keyword { get; set; }
        
        public bool IncludeChildren { get; set; } = false;
    }

    /// <summary>
    /// 博客分类树形DTO
    /// </summary>
    public class BlogCategoryTreeDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public Guid? ParentId { get; set; }
        
        public int SortOrder { get; set; }
        
        public string? Icon { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; }
        
        public int PostCount { get; set; }
        
        public List<BlogCategoryTreeDto> Children { get; set; } = new List<BlogCategoryTreeDto>();
    }
}
