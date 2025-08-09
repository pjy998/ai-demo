using Volo.Abp.Settings;

namespace BlogBackend.Settings;

public class BlogBackendSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BlogBackendSettings.MySetting1));
    }
}
