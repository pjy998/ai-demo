using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客标签DTO
    /// </summary>
    public class BlogTagDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Color { get; set; }
        
        public int UsageCount { get; set; }
        
        public bool IsActive { get; set; }
        
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 博客标签简化DTO
    /// </summary>
    public class BlogTagBriefDto : EntityDto<Guid>
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? Color { get; set; }
        
        public int UsageCount { get; set; }
        
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 创建博客标签DTO
    /// </summary>
    public class CreateBlogTagDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 更新博客标签DTO
    /// </summary>
    public class UpdateBlogTagDto
    {
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Color { get; set; }
        
        public bool IsActive { get; set; }
        
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 博客标签查询DTO
    /// </summary>
    public class GetBlogTagListDto : PagedAndSortedResultRequestDto
    {
        public bool? IsActive { get; set; }
        
        public string? Keyword { get; set; }
        
        public int? MinUsageCount { get; set; }
    }

    /// <summary>
    /// 博客标签统计DTO
    /// </summary>
    public class BlogTagStatisticsDto
    {
        public int TotalCount { get; set; }
        
        public int ActiveCount { get; set; }
        
        public int UnusedCount { get; set; }
        
        public List<BlogTagBriefDto> PopularTags { get; set; } = new List<BlogTagBriefDto>();
    }
}
