using BlogBackend.Samples;
using Xunit;

namespace BlogBackend.EntityFrameworkCore.Domains;

[Collection(BlogBackendTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<BlogBackendEntityFrameworkCoreTestModule>
{

}
