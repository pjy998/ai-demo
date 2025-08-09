using Volo.Abp.Modularity;

namespace BlogBackend;

public abstract class BlogBackendApplicationTestBase<TStartupModule> : BlogBackendTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
