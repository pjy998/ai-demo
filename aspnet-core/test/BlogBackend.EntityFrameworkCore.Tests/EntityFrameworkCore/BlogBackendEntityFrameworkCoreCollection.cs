using Xunit;

namespace BlogBackend.EntityFrameworkCore;

[CollectionDefinition(BlogBackendTestConsts.CollectionDefinitionName)]
public class BlogBackendEntityFrameworkCoreCollection : ICollectionFixture<BlogBackendEntityFrameworkCoreFixture>
{

}
