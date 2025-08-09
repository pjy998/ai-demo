using BlogBackend.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace BlogBackend.Permissions;

public class BlogBackendPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BlogBackendPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(BlogBackendPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BlogBackendResource>(name);
    }
}
