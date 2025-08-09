using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlogBackend.Data;
using Volo.Abp.DependencyInjection;

namespace BlogBackend.EntityFrameworkCore;

public class EntityFrameworkCoreBlogBackendDbSchemaMigrator
    : IBlogBackendDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreBlogBackendDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the BlogBackendDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<BlogBackendDbContext>()
            .Database
            .MigrateAsync();
    }
}
