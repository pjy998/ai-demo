using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BlogBackend.Data;

/* This is used if database provider does't define
 * IBlogBackendDbSchemaMigrator implementation.
 */
public class NullBlogBackendDbSchemaMigrator : IBlogBackendDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
