using Microsoft.Extensions.Localization;
using BlogBackend.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace BlogBackend;

[Dependency(ReplaceServices = true)]
public class BlogBackendBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<BlogBackendResource> _localizer;

    public BlogBackendBrandingProvider(IStringLocalizer<BlogBackendResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
