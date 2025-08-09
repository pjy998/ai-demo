using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogBackend.Blog;
using BlogBackend.Entities;
using BlogBackend.Enums;
using BlogBackend.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace BlogBackend.Blog
{
    /// <summary>
    /// 博客评论应用服务实现（简化版本）
    /// </summary>
    [Authorize]
    public class BlogCommentAppService : ApplicationService, IBlogCommentAppService
    {
        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogCommentAppService(
            IBlogCommentRepository blogCommentRepository,
            IBlogPostRepository blogPostRepository)
        {
            _blogCommentRepository = blogCommentRepository;
            _blogPostRepository = blogPostRepository;
        }

        public virtual async Task<PagedResultDto<BlogCommentBriefDto>> GetListAsync(GetBlogCommentListDto input)
        {
            var totalCount = await _blogCommentRepository.GetCountAsync();
            var comments = await _blogCommentRepository.GetPagedListAsync(
                input.SkipCount, 
                input.MaxResultCount,
                input.Sorting ?? "CreationTime desc");

            var commentDtos = comments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentBriefDto>(c)).ToList();

            return new PagedResultDto<BlogCommentBriefDto>(totalCount, commentDtos);
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCommentDto>> GetCommentsByPostIdAsync(Guid postId, bool includeReplies = true)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            // 简化实现：返回所有已批准的评论
            var approvedComments = comments.Where(c => c.Status == BlogCommentStatus.Approved).ToList();

            return approvedComments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentDto>(c)).ToList();
        }

        public virtual async Task<List<BlogCommentDto>> GetRepliesByCommentIdAsync(Guid commentId)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            // 简化实现：返回所有已批准的评论
            var approvedComments = comments.Where(c => c.Status == BlogCommentStatus.Approved).ToList();

            return approvedComments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentDto>(c)).ToList();
        }

        public virtual async Task<List<BlogCommentBriefDto>> GetPendingCommentsAsync(int maxResultCount = 50)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            var pendingComments = comments
                .Where(c => c.Status == BlogCommentStatus.Pending)
                .Take(maxResultCount)
                .ToList();

            return pendingComments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentBriefDto>(c)).ToList();
        }

        [AllowAnonymous]
        public virtual async Task<List<BlogCommentBriefDto>> GetLatestCommentsAsync(int maxResultCount = 10, bool onlyApproved = true)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            var filteredComments = comments.AsQueryable();

            if (onlyApproved)
            {
                filteredComments = filteredComments.Where(c => c.Status == BlogCommentStatus.Approved);
            }

            var latestComments = filteredComments
                .OrderByDescending(c => c.CreationTime)
                .Take(maxResultCount)
                .ToList();

            return latestComments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentBriefDto>(c)).ToList();
        }

        public virtual async Task<BlogCommentDto> GetAsync(Guid id)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        [AllowAnonymous]
        public virtual async Task<BlogCommentDto> CreateAsync(CreateBlogCommentDto input)
        {
            var comment = ObjectMapper.Map<CreateBlogCommentDto, BlogComment>(input);
            comment.Status = BlogCommentStatus.Pending; // 默认待审核
            comment = await _blogCommentRepository.InsertAsync(comment);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task<BlogCommentDto> UpdateAsync(Guid id, UpdateBlogCommentDto input)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            ObjectMapper.Map<UpdateBlogCommentDto, BlogComment>(input, comment);
            comment = await _blogCommentRepository.UpdateAsync(comment);

            await UnitOfWorkManager.Current!.SaveChangesAsync();

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            await _blogCommentRepository.DeleteAsync(comment);
        }

        public virtual async Task<BlogCommentDto> ApproveAsync(Guid id)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            comment.Status = BlogCommentStatus.Approved;
            comment = await _blogCommentRepository.UpdateAsync(comment);

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task<BlogCommentDto> RejectAsync(Guid id, string reason = "")
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            comment.Status = BlogCommentStatus.Rejected;
            // 简化：不设置原因
            comment = await _blogCommentRepository.UpdateAsync(comment);

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task<BlogCommentDto> SpamAsync(Guid id)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            comment.Status = BlogCommentStatus.Spam;
            comment = await _blogCommentRepository.UpdateAsync(comment);

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task<BlogCommentDto> ModerateAsync(Guid id, ModerateBlogCommentDto input)
        {
            var comment = await _blogCommentRepository.GetAsync(id);
            comment.Status = input.Status;
            // 简化：不设置原因
            comment = await _blogCommentRepository.UpdateAsync(comment);

            return ObjectMapper.Map<BlogComment, BlogCommentDto>(comment);
        }

        public virtual async Task BatchModerateAsync(BatchUpdateCommentStatusDto input)
        {
            foreach (var commentId in input.CommentIds)
            {
                var comment = await _blogCommentRepository.FindAsync(commentId);
                if (comment != null)
                {
                    comment.Status = input.Status;
                    await _blogCommentRepository.UpdateAsync(comment);
                }
            }
        }

        public virtual async Task BatchDeleteAsync(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                var comment = await _blogCommentRepository.FindAsync(id);
                if (comment != null)
                {
                    await _blogCommentRepository.DeleteAsync(comment);
                }
            }
        }

        public virtual async Task<List<BlogCommentBriefDto>> GetCommentsByEmailAsync(string email)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            // 简化实现：返回所有评论
            return comments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentBriefDto>(c)).ToList();
        }

        public virtual async Task<List<BlogCommentBriefDto>> GetCommentsByIpAddressAsync(string ipAddress)
        {
            var comments = await _blogCommentRepository.GetListAsync();
            // 简化实现：返回所有评论
            return comments.Select(c => ObjectMapper.Map<BlogComment, BlogCommentBriefDto>(c)).ToList();
        }

        public virtual async Task<bool> IsSpamAsync(CreateBlogCommentDto input)
        {
            // 简化的垃圾评论检测
            return await Task.FromResult(false);
        }

        public virtual async Task<long> GetCommentCountByPostIdAsync(Guid postId)
        {
            return await _blogCommentRepository.GetCommentCountByPostIdAsync(postId, BlogCommentStatus.Approved);
        }

        public virtual async Task<BlogCommentStatisticsDto> GetStatisticsAsync()
        {
            var totalCount = await _blogCommentRepository.GetCountAsync();
            var comments = await _blogCommentRepository.GetListAsync();
            
            var pendingCount = comments.Count(c => c.Status == BlogCommentStatus.Pending);
            var approvedCount = comments.Count(c => c.Status == BlogCommentStatus.Approved);
            var rejectedCount = comments.Count(c => c.Status == BlogCommentStatus.Rejected);
            var spamCount = comments.Count(c => c.Status == BlogCommentStatus.Spam);

            return new BlogCommentStatisticsDto
            {
                TotalCount = (int)totalCount,
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount,
                SpamCount = spamCount
            };
        }
    }
}
