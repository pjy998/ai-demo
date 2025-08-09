using BlogBackend.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BlogBackendController : AbpControllerBase
{
    protected BlogBackendController()
    {
        LocalizationResource = typeof(BlogBackendResource);
    }
}
