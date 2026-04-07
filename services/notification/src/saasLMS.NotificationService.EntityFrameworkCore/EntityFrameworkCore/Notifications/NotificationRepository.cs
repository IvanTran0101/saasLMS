using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.NotificationService.Notifications;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.NotificationService.EntityFrameworkCore.Notifications;

public class NotificationRepository
    : EfCoreRepository<NotificationServiceDbContext, Notification, Guid>,
        INotificationRepository
{
    public NotificationRepository(
        IDbContextProvider<NotificationServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Notification>> GetListByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .Where(n => n.TenantId == tenantId
                        && n.UserId == userId)
            .OrderByDescending(n => n.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Notification>> GetUnreadListByUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(n => n.TenantId == tenantId
                        && n.UserId == userId
                        && !n.IsRead)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .CountAsync(
                n => n.TenantId == tenantId
                     && n.UserId == userId
                     && !n.IsRead,
                cancellationToken);
    }
}