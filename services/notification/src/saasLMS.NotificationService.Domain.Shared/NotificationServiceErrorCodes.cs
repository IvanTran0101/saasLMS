// Domain.Shared/NotificationServiceErrorCodes.cs
namespace saasLMS.NotificationService;

public static class NotificationServiceErrorCodes
{
    // Pattern: "Notification:{ErrorName}"
    public const string TenantNotFound        = "Notification:TenantNotFound";
    public const string NotificationNotFound  = "Notification:NotificationNotFound";
    public const string UnauthorizedAccess    = "Notification:UnauthorizedAccess";
    public const string InvalidRecipient = "Notification:InvalidRecipient";
}