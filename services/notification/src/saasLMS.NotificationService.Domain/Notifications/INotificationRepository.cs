using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.NotificationService.Notifications;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<List<Notification>> GetListByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<List<Notification>> GetUnreadListByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
}