using saasLMS.EnrollmentService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.EnrollmentService.Permissions;

public class EnrollmentServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(
            EnrollmentServicePermissions.GroupName,
            L("Permission:EnrollmentService"));

        var enrollments = myGroup.AddPermission(
            EnrollmentServicePermissions.Enrollments.Default,
            L("Permission:Enrollments"));

        enrollments.AddChild(EnrollmentServicePermissions.Enrollments.Create,  L("Permission:Create"));
        enrollments.AddChild(EnrollmentServicePermissions.Enrollments.Cancel,  L("Permission:Cancel"));
        enrollments.AddChild(EnrollmentServicePermissions.Enrollments.View,    L("Permission:View"));
        enrollments.AddChild(EnrollmentServicePermissions.Enrollments.ViewOwn, L("Permission:ViewOwn"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EnrollmentServiceResource>(name);
    }
}
