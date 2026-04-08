using System;

namespace saasLMS.NotificationService.Notifications.Dtos.Inputs;

public class SendNotificationInput
{
    public Guid EventId { get; set; }
    public Guid TenantId { get; set; }
    public Guid RecipientUserId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public NotificationType Type { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
}