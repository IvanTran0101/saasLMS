using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.CourseCatalogService.Permissions;

public class CourseCatalogServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CourseCatalogServicePermissions.GroupName, L("Permission:CourseCatalogService"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BookStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CourseCatalogServiceResource>(name);
    }
}
