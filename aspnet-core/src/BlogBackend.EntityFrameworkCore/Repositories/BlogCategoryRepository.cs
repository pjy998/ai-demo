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
    public class BlogCategoryRepository : EfCoreRepository<BlogBackendDbContext, BlogCategory, Guid>, IBlogCategoryRepository
    {
        public BlogCategoryRepository(IDbContextProvider<BlogBackendDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<BlogCategory?> FindBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogCategories
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogCategories.Where(x => x.Slug == slug);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            
            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<BlogCategory>> GetRootCategoriesAsync(
            bool onlyActive = true,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogCategories
                .Where(x => x.ParentId == null);

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogCategory>> GetChildCategoriesAsync(
            Guid parentId,
            bool onlyActive = true,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogCategories
                .Where(x => x.ParentId == parentId);

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<BlogCategory>> GetCategoryTreeAsync(
            bool onlyActive = true,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var query = dbContext.BlogCategories.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            var categories = await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            // 构建树形结构
            var categoryDict = categories.ToDictionary(x => x.Id);
            var rootCategories = new List<BlogCategory>();

            foreach (var category in categories)
            {
                if (category.ParentId == null)
                {
                    rootCategories.Add(category);
                }
                else if (categoryDict.ContainsKey(category.ParentId.Value))
                {
                    var parent = categoryDict[category.ParentId.Value];
                    if (parent.Children == null)
                        parent.Children = new List<BlogCategory>();
                    parent.Children.Add(category);
                }
            }

            return rootCategories;
        }

        public async Task<List<BlogCategory>> GetCategoryPathAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var path = new List<BlogCategory>();
            var currentId = (Guid?)categoryId;

            while (currentId.HasValue)
            {
                var category = await dbContext.BlogCategories
                    .FirstOrDefaultAsync(x => x.Id == currentId.Value, cancellationToken);

                if (category == null)
                    break;

                path.Insert(0, category);
                currentId = category.ParentId;
            }

            return path;
        }

        public async Task<List<Guid>> GetDescendantIdsAsync(
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var descendantIds = new List<Guid>();
            var currentLevelIds = new List<Guid> { parentId };

            while (currentLevelIds.Count > 0)
            {
                var childIds = await dbContext.BlogCategories
                    .Where(x => x.ParentId != null && currentLevelIds.Contains(x.ParentId.Value))
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken);

                descendantIds.AddRange(childIds);
                currentLevelIds = childIds;
            }

            return descendantIds;
        }

        public async Task<bool> HasChildrenAsync(
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogCategories
                .AnyAsync(x => x.ParentId == parentId, cancellationToken);
        }

        public async Task<bool> HasPostsAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogPosts
                .AnyAsync(x => x.CategoryId == categoryId, cancellationToken);
        }

        public async Task<List<BlogCategory>> GetActiveCategoriesAsync(
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.BlogCategories
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> WillCreateCircularReferenceAsync(
            Guid categoryId,
            Guid parentId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            var currentId = (Guid?)parentId;

            while (currentId.HasValue)
            {
                if (currentId.Value == categoryId)
                    return true;

                var parent = await dbContext.BlogCategories
                    .FirstOrDefaultAsync(x => x.Id == currentId.Value, cancellationToken);

                currentId = parent?.ParentId;
            }

            return false;
        }
    }
}
