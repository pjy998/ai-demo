using BlogBackend.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace BlogBackend.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BlogBackendEntityFrameworkCoreModule),
    typeof(BlogBackendApplicationContractsModule)
    )]
public class BlogBackendDbMigratorModule : AbpModule
{
}
