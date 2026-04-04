using saasLMS.LearningProgressService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.LearningProgressService.Permissions;

public class LearningProgressServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(LearningProgressServicePermissions.GroupName, L("Permission:LearningProgressService"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BookStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LearningProgressServiceResource>(name);
    }
}
