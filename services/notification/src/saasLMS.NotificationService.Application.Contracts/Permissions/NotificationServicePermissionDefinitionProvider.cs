using saasLMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.NotificationService.Permissions;

public class NotificationServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NotificationServicePermissions.GroupName, L("Permission:NotificationService"));

        var notifications = myGroup.AddPermission(
            NotificationServicePermissions.Notifications.Default,
            L("Permission:Notifications"));

        notifications.AddChild(
            NotificationServicePermissions.Notifications.ViewMy,
            L("Permission:ViewOwn"));

        notifications.AddChild(
            NotificationServicePermissions.Notifications.Manage,
            L("Permission:Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<saasLMSResource>(name);
    }
}
