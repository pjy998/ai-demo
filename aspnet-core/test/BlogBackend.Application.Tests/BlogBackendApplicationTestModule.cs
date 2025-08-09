using Volo.Abp.Modularity;

namespace BlogBackend;

[DependsOn(
    typeof(BlogBackendApplicationModule),
    typeof(BlogBackendDomainTestModule)
)]
public class BlogBackendApplicationTestModule : AbpModule
{

}
