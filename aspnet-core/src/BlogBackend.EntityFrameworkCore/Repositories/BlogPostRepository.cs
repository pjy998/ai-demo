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
    public class BlogPostRepository : EfCoreRepository<BlogBackendDbContext, BlogPost, Guid>, IBlogPostRepository
    {
        public BlogPostRepository(IDbContextProvider<BlogBackendDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<BlogPost?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPosts
                .Include(x => x.Category)
                .Include(x => x.Tags).ThenInclude(x => x.BlogTag)
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts.Where(x => x.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetPagedListAsync(
            int skipCount,
            int maxResultCount,
            BlogPostStatus? status = null,
            Guid? categoryId = null,
            string? keyword = null,
            string sorting = "creationTime desc",
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts
                .Include(x => x.Category)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Title, keyword) || 
                                       EF.Functions.Like(x.Summary, keyword) || 
                                       EF.Functions.Like(x.Content, keyword));
            }

            // 简单排序实现
            query = sorting.ToLower() switch
            {
                "title" => query.OrderBy(x => x.Title),
                "title desc" => query.OrderByDescending(x => x.Title),
                "publishedtime" => query.OrderBy(x => x.PublishedTime),
                "publishedtime desc" => query.OrderByDescending(x => x.PublishedTime),
                "viewcount" => query.OrderBy(x => x.ViewCount),
                "viewcount desc" => query.OrderByDescending(x => x.ViewCount),
                "creationtime" => query.OrderBy(x => x.CreationTime),
                _ => query.OrderByDescending(x => x.CreationTime)
            };

            return await query
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            BlogPostStatus? status = null,
            Guid? categoryId = null,
            string? keyword = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Title, keyword) || 
                                       EF.Functions.Like(x.Summary, keyword) || 
                                       EF.Functions.Like(x.Content, keyword));
            }

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetPublishedPostsAsync(
            int skipCount,
            int maxResultCount,
            Guid? categoryId = null,
            List<Guid>? tagIds = null,
            string? keyword = null,
            string sorting = "publishedTime desc",
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts
                .Include(x => x.Category)
                .Where(x => x.Status == BlogPostStatus.Published && x.PublishedTime <= DateTime.Now);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (tagIds != null && tagIds.Count > 0)
            {
                query = query.Where(x => x.Tags.Any(t => tagIds.Contains(t.BlogTagId)));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Title, keyword) || 
                                       EF.Functions.Like(x.Summary, keyword));
            }

            // 排序
            query = sorting.ToLower() switch
            {
                "title" => query.OrderBy(x => x.Title),
                "title desc" => query.OrderByDescending(x => x.Title),
                "viewcount" => query.OrderBy(x => x.ViewCount),
                "viewcount desc" => query.OrderByDescending(x => x.ViewCount),
                "publishedtime" => query.OrderBy(x => x.PublishedTime),
                _ => query.OrderByDescending(x => x.PublishedTime)
            };

            return await query
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetPublishedCountAsync(
            Guid? categoryId = null,
            List<Guid>? tagIds = null,
            string? keyword = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts
                .Where(x => x.Status == BlogPostStatus.Published && x.PublishedTime <= DateTime.Now);

            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (tagIds != null && tagIds.Count > 0)
            {
                query = query.Where(x => x.Tags.Any(t => tagIds.Contains(t.BlogTagId)));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Title, keyword) || 
                                       EF.Functions.Like(x.Summary, keyword));
            }

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetRelatedPostsAsync(
            Guid blogPostId,
            int maxResultCount = 5,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var post = await dbContext.BlogPosts
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == blogPostId, cancellationToken);

            if (post == null)
                return new List<BlogPost>();

            var tagIds = post.Tags.Select(x => x.BlogTagId).ToList();

            var query = dbContext.BlogPosts
                .Include(x => x.Category)
                .Where(x => x.Id != blogPostId && 
                           x.Status == BlogPostStatus.Published && 
                           x.PublishedTime <= DateTime.Now);

            if (post.CategoryId.HasValue)
            {
                // 优先同分类
                query = query.Where(x => x.CategoryId == post.CategoryId);
            }

            if (tagIds.Count > 0)
            {
                // 按共同标签数排序
                query = query.OrderByDescending(x => x.Tags.Count(t => tagIds.Contains(t.BlogTagId)));
            }

            return await query
                .OrderByDescending(x => x.ViewCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetPopularPostsAsync(
            int maxResultCount = 10,
            int? dayCount = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogPosts
                .Include(x => x.Category)
                .Where(x => x.Status == BlogPostStatus.Published && x.PublishedTime <= DateTime.Now);

            if (dayCount.HasValue)
            {
                var fromDate = DateTime.Now.AddDays(-dayCount.Value);
                query = query.Where(x => x.PublishedTime >= fromDate);
            }

            return await query
                .OrderByDescending(x => x.ViewCount)
                .ThenByDescending(x => x.LikeCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogPost>> GetLatestPostsAsync(
            int maxResultCount = 10,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPosts
                .Include(x => x.Category)
                .Where(x => x.Status == BlogPostStatus.Published && x.PublishedTime <= DateTime.Now)
                .OrderByDescending(x => x.PublishedTime)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<BlogPost?> GetWithTagsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPosts
                .Include(x => x.Category)
                .Include(x => x.Tags).ThenInclude(x => x.BlogTag)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            await dbContext.BlogPosts
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.ViewCount, p => p.ViewCount + 1), cancellationToken);
        }

        public async Task IncrementLikeCountAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            await dbContext.BlogPosts
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.LikeCount, p => p.LikeCount + 1), cancellationToken);
        }

        public async Task<Dictionary<Guid, int>> GetPostCountByCategoryAsync(
            List<Guid> categoryIds,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPosts
                .Where(x => categoryIds.Contains(x.CategoryId.Value) && x.Status == BlogPostStatus.Published)
                .GroupBy(x => x.CategoryId.Value)
                .ToDictionaryAsync(x => x.Key, x => x.Count(), cancellationToken);
        }

        public async Task<Dictionary<Guid, int>> GetPostCountByTagAsync(
            List<Guid> tagIds,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPostTags
                .Include(x => x.BlogPost)
                .Where(x => tagIds.Contains(x.BlogTagId) && x.BlogPost.Status == BlogPostStatus.Published)
                .GroupBy(x => x.BlogTagId)
                .ToDictionaryAsync(x => x.Key, x => x.Count(), cancellationToken);
        }
    }
}
