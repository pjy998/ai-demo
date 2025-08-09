using AutoMapper;
using BlogBackend.Blog;
using BlogBackend.Entities;
using System.Linq;
using System.Collections.Generic;

namespace BlogBackend;

public class BlogBackendApplicationAutoMapperProfile : Profile
{
    public BlogBackendApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        // BlogPost mappings
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(pt => pt.BlogTag).ToList()));
            
        CreateMap<BlogPost, BlogPostBriefDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.TagNames, opt => opt.MapFrom(src => src.Tags.Select(pt => pt.BlogTag.Name).ToList()));
            
        CreateMap<CreateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BlogBackend.Enums.BlogPostStatus.Draft))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => 0));
            
        CreateMap<UpdateBlogPostDto, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.LikeCount, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedTime, opt => opt.Ignore());

        // BlogCategory mappings
        CreateMap<BlogCategory, BlogCategoryDto>()
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts != null ? src.Posts.Count : 0));
            
        CreateMap<BlogCategory, BlogCategoryBriefDto>()
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts != null ? src.Posts.Count : 0));
            
        CreateMap<BlogCategory, BlogCategoryTreeDto>()
            .ForMember(dest => dest.PostCount, opt => opt.MapFrom(src => src.Posts != null ? src.Posts.Count : 0));
            
        CreateMap<CreateBlogCategoryDto, BlogCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            
        CreateMap<UpdateBlogCategoryDto, BlogCategory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // BlogTag mappings
        CreateMap<BlogTag, BlogTagDto>();
        CreateMap<BlogTag, BlogTagBriefDto>();
        CreateMap<CreateBlogTagDto, BlogTag>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsageCount, opt => opt.MapFrom(src => 0));
        CreateMap<UpdateBlogTagDto, BlogTag>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsageCount, opt => opt.Ignore());

        // BlogComment mappings
        CreateMap<BlogComment, BlogCommentDto>()
            .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.ToList() : new List<BlogComment>()));
            
        CreateMap<BlogComment, BlogCommentBriefDto>()
            .ForMember(dest => dest.PostTitle, opt => opt.MapFrom(src => src.BlogPost != null ? src.BlogPost.Title : null))
            .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count : 0));
            
        CreateMap<CreateBlogCommentDto, BlogComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BlogBackend.Enums.BlogCommentStatus.Pending));
            
        CreateMap<UpdateBlogCommentDto, BlogComment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPostId, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCommentId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
            .ForMember(dest => dest.UserAgent, opt => opt.Ignore());
    }
}
