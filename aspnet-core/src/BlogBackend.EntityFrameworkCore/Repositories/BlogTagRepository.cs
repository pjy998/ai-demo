using BlogBackend.Entities;
using BlogBackend.EntityFrameworkCore;
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
    public class BlogTagRepository : EfCoreRepository<BlogBackendDbContext, BlogTag, Guid>, IBlogTagRepository
    {
        public BlogTagRepository(IDbContextProvider<BlogBackendDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<BlogTag?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogTags
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task<BlogTag?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogTags
                .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogTags.Where(x => x.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogTags.Where(x => x.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<BlogTag>> GetListAsync(
            bool onlyActive = true,
            string? keyword = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogTags.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = $"%{keyword}%";
                query = query.Where(x => EF.Functions.Like(x.Name, keyword) || 
                                       EF.Functions.Like(x.Description, keyword));
            }

            return await query
                .OrderByDescending(x => x.UsageCount)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogTag>> GetPopularTagsAsync(
            int maxResultCount = 20,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogTags
                .Where(x => x.IsActive && x.UsageCount > 0)
                .OrderByDescending(x => x.UsageCount)
                .ThenBy(x => x.Name)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogTag>> GetOrCreateTagsByNamesAsync(
            List<string> tagNames,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var existingTags = await dbContext.BlogTags
                .Where(x => tagNames.Contains(x.Name))
                .ToListAsync(cancellationToken);

            var existingTagNames = existingTags.Select(x => x.Name).ToList();
            var newTagNames = tagNames.Except(existingTagNames, StringComparer.OrdinalIgnoreCase).ToList();

            var newTags = new List<BlogTag>();
            foreach (var tagName in newTagNames)
            {
                var tag = BlogTag.Create(tagName, tagName.ToLower().Replace(" ", "-"));
                newTags.Add(tag);
                await dbContext.BlogTags.AddAsync(tag, cancellationToken);
            }

            if (newTags.Count > 0)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            var result = new List<BlogTag>();
            result.AddRange(existingTags);
            result.AddRange(newTags);

            return result;
        }

        public async Task<List<BlogTag>> GetTagsByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPostTags
                .Include(x => x.BlogTag)
                .Where(x => x.BlogPostId == postId)
                .Select(x => x.BlogTag)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateUsageCountAsync(
            Guid tagId,
            int increment = 1,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            await dbContext.BlogTags
                .Where(x => x.Id == tagId)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.UsageCount, p => p.UsageCount + increment), cancellationToken);
        }

        public async Task BatchUpdateUsageCountAsync(
            Dictionary<Guid, int> tagUsageChanges,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            
            foreach (var kvp in tagUsageChanges)
            {
                await dbContext.BlogTags
                    .Where(x => x.Id == kvp.Key)
                    .ExecuteUpdateAsync(x => x.SetProperty(p => p.UsageCount, p => p.UsageCount + kvp.Value), cancellationToken);
            }
        }

        public async Task<List<BlogTag>> GetUnusedTagsAsync(
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogTags
                .Where(x => x.UsageCount == 0)
                .OrderBy(x => x.CreationTime)
                .ToListAsync(cancellationToken);
        }
    }
}
