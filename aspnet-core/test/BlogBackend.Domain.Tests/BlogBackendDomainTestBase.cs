using Volo.Abp.Modularity;

namespace BlogBackend;

/* Inherit from this class for your domain layer tests. */
public abstract class BlogBackendDomainTestBase<TStartupModule> : BlogBackendTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
