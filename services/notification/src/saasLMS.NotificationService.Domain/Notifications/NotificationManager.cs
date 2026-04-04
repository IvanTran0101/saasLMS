using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.NotificationService.Notifications;

public class NotificationManager : DomainService
{
    public Task<Notification> CreateAsync(
        Guid tenantId,
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? referenceType = null,
        string? referenceId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification(
            GuidGenerator.Create(),
            tenantId,
            userId,
            title,
            message,
            type,
            referenceType,
            referenceId);
        return Task.FromResult(notification);
    }

    public Task MarkAsReadAsync(
        Notification notification,
        DateTime readAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(notification, nameof(notification));
        notification.MarkAsRead(readAt);
        return Task.CompletedTask;
    }
    
    public Task MarkAsUnreadAsync(
        Notification notification,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(notification, nameof(notification));
        notification.MarkAsUnread();
        return Task.CompletedTask;
    }
}