using System.Collections.Generic;

namespace saasLMS.NotificationService.Notifications.Dtos.Outputs;

public class NotificationSummaryDto
{
    public List<NotificationDto> Items { get; set; } = [];
    public int UnreadCount { get; set; }
}