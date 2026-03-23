using saasLMS.EnrollmentService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.EnrollmentService.Permissions;

public class EnrollmentServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(EnrollmentServicePermissions.GroupName, L("Permission:EnrollmentService"));

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BookStorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EnrollmentServiceResource>(name);
    }
}
