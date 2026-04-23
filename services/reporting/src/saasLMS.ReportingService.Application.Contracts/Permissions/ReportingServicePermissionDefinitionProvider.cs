using saasLMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.ReportingService.Permissions;

public class ReportingServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ReportingServicePermissions.GroupName, L("Permission:ReportingService"));

        myGroup.AddPermission(ReportingServicePermissions.Reports.View, L("Permission:Reports.View"));
        myGroup.AddPermission(ReportingServicePermissions.Reports.StudentView, L("Permission:Reports.StudentView"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<saasLMSResource>(name);
    }
}
