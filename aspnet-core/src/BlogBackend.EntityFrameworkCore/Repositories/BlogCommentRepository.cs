using BlogBackend.Entities;
using BlogBackend.EntityFrameworkCore;
using BlogBackend.Enums;
using BlogBackend.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BlogBackend.EntityFrameworkCore.Repositories
{
    public class BlogCommentRepository : EfCoreRepository<BlogBackendDbContext, BlogComment, Guid>, IBlogCommentRepository
    {
        public BlogCommentRepository(IDbContextProvider<BlogBackendDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<BlogComment>> GetCommentsByPostIdAsync(
            Guid postId,
            BlogCommentStatus? status = null,
            bool includeReplies = true,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments
                .Where(x => x.BlogPostId == postId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (includeReplies)
            {
                query = query.Include(x => x.Replies.Where(r => !status.HasValue || r.Status == status.Value));
            }
            else
            {
                query = query.Where(x => x.ParentCommentId == null);
            }

            return await query
                .OrderBy(x => x.CreationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetCommentCountByPostIdAsync(
            Guid postId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments
                .Where(x => x.BlogPostId == postId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetRepliesByCommentIdAsync(
            Guid parentCommentId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments
                .Where(x => x.ParentCommentId == parentCommentId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query
                .OrderBy(x => x.CreationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetReplyCountByCommentIdAsync(
            Guid parentCommentId,
            BlogCommentStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments
                .Where(x => x.ParentCommentId == parentCommentId);

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            Guid? postId = null,
            BlogCommentStatus? status = null,
            string? keyword = null,
            string sorting = "creationTime desc",
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments
                .Include(x => x.BlogPost)
                .AsQueryable();

            if (postId.HasValue)
            {
                query = query.Where(x => x.BlogPostId == postId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Content, keyword) ||
                                       EF.Functions.Like(x.AuthorName, keyword) ||
                                       EF.Functions.Like(x.AuthorEmail, keyword));
            }

            // 排序
            query = sorting.ToLower() switch
            {
                "authorname" => query.OrderBy(x => x.AuthorName),
                "authorname desc" => query.OrderByDescending(x => x.AuthorName),
                "creationtime" => query.OrderBy(x => x.CreationTime),
                _ => query.OrderByDescending(x => x.CreationTime)
            };

            return await query
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            Guid? postId = null,
            BlogCommentStatus? status = null,
            string? keyword = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments.AsQueryable();

            if (postId.HasValue)
            {
                query = query.Where(x => x.BlogPostId == postId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Content, keyword) ||
                                       EF.Functions.Like(x.AuthorName, keyword) ||
                                       EF.Functions.Like(x.AuthorEmail, keyword));
            }

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetPendingCommentsAsync(
            int maxResultCount = 50,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogComments
                .Include(x => x.BlogPost)
                .Where(x => x.Status == BlogCommentStatus.Pending)
                .OrderBy(x => x.CreationTime)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetPendingCommentCountAsync(
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogComments
                .Where(x => x.Status == BlogCommentStatus.Pending)
                .LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetLatestCommentsAsync(
            int maxResultCount = 10,
            bool onlyApproved = true,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogComments.AsQueryable();

            if (onlyApproved)
            {
                query = query.Where(x => x.Status == BlogCommentStatus.Approved);
            }

            return await query
                .Include(x => x.BlogPost)
                .OrderByDescending(x => x.CreationTime)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetCommentsByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogComments
                .Include(x => x.BlogPost)
                .Where(x => x.AuthorEmail == email)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogComment>> GetCommentsByIpAddressAsync(
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogComments
                .Include(x => x.BlogPost)
                .Where(x => x.IpAddress == ipAddress)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync(cancellationToken);
        }

        public async Task BatchUpdateStatusAsync(
            List<Guid> commentIds,
            BlogCommentStatus status,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            await dbContext.BlogComments
                .Where(x => commentIds.Contains(x.Id))
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.Status, status), cancellationToken);
        }

        public async Task DeleteByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            await dbContext.BlogComments
                .Where(x => x.BlogPostId == postId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<Dictionary<BlogCommentStatus, int>> GetCommentStatisticsAsync(
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogComments
                .GroupBy(x => x.Status)
                .ToDictionaryAsync(x => x.Key, x => x.Count(), cancellationToken);
        }
    }
}
