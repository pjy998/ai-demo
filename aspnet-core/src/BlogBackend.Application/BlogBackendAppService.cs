using System;
using System.Collections.Generic;
using System.Text;
using BlogBackend.Localization;
using Volo.Abp.Application.Services;

namespace BlogBackend;

/* Inherit your application services from this class.
 */
public abstract class BlogBackendAppService : ApplicationService
{
    protected BlogBackendAppService()
    {
        LocalizationResource = typeof(BlogBackendResource);
    }
}
