using System;

namespace saasLMS.NotificationService.Notifications.Dtos.Inputs;

public class SendNotificationInput
{
    public Guid EventId { get; set; }
    public Guid TenantId { get; set; }
    public Guid RecipientUserId { get; set; }
    //To do: Cần được resolve từ Identity Service trước khi gọi SendNotificationAsync.
    //Hiện tại các ETO Handler truyền null — email channel sẽ bị skip nếu null.
    public string? RecipientEmail { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public NotificationType Type { get; set; }
    public string? ReferenceType { get; set; }
    public string? ReferenceId { get; set; }
}