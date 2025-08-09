using BlogBackend.Samples;
using Xunit;

namespace BlogBackend.EntityFrameworkCore.Applications;

[Collection(BlogBackendTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<BlogBackendEntityFrameworkCoreTestModule>
{

}
