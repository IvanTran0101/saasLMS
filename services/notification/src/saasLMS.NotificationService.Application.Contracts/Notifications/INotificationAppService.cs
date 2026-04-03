using System;
using System.Threading.Tasks;
using saasLMS.NotificationService.Notifications.Dtos.Inputs;
using saasLMS.NotificationService.Notifications.Dtos.Outputs;
using Volo.Abp.Application.Services;

namespace saasLMS.NotificationService.Notifications;

public interface INotificationAppService : IApplicationService
{
    Task<NotificationSummaryDto> GetMyNotificationsAsync();
    Task<int> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync();
    Task SendNotificationAsync(SendNotificationInput input);
}