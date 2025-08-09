using Volo.Abp.Modularity;

namespace BlogBackend;

[DependsOn(
    typeof(BlogBackendDomainModule),
    typeof(BlogBackendTestBaseModule)
)]
public class BlogBackendDomainTestModule : AbpModule
{

}
