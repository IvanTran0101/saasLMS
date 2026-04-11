using Volo.Abp.Reflection;

namespace saasLMS.NotificationService.Permissions;

public class NotificationServicePermissions
{
    public const string GroupName = "NotificationService";
    
    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string ViewOwn = Default + ".ViewOwn"; // Xem thông báo của mình
        public const string Manage  = Default + ".Manage";  // Đánh dấu đã đọc
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NotificationServicePermissions));
    }
}
