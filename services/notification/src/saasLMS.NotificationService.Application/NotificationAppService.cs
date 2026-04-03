using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.NotificationService.Notifications;
using saasLMS.NotificationService.Notifications.Dtos.Inputs;
using saasLMS.NotificationService.Notifications.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace saasLMS.NotificationService;

public class NotificationAppService : NotificationServiceAppService, INotificationAppService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationManager _notificationManager;

    public NotificationAppService(
        INotificationRepository notificationRepository,
        NotificationManager notificationManager)
    {
        _notificationRepository = notificationRepository;
        _notificationManager = notificationManager;
    }

    public async Task<NotificationSummaryDto> GetMyNotificationsAsync()
    {
        var tenantId = CurrentTenant.Id
            ?? throw new BusinessException(NotificationServiceErrorCodes.TenantNotFound);

        var userId = CurrentUser.GetId();

        var items = await _notificationRepository.GetListByUserAsync(tenantId, userId);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(tenantId, userId);

        return new NotificationSummaryDto
        {
            Items = ObjectMapper.Map<List<Notification>, List<NotificationDto>>(items),
            UnreadCount = unreadCount
        };
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var tenantId = CurrentTenant.Id
            ?? throw new BusinessException(NotificationServiceErrorCodes.TenantNotFound);

        var userId = CurrentUser.GetId();

        return await _notificationRepository.GetUnreadCountAsync(tenantId, userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var tenantId = CurrentTenant.Id
            ?? throw new BusinessException(NotificationServiceErrorCodes.TenantNotFound);

        var userId = CurrentUser.GetId();

        var notification = await _notificationRepository.GetAsync(notificationId);

        if (notification.TenantId != tenantId || notification.UserId != userId)
        {
            throw new BusinessException(NotificationServiceErrorCodes.NotificationNotFound);
        }

        await _notificationManager.MarkAsReadAsync(notification, Clock.Now);
        await _notificationRepository.UpdateAsync(notification, autoSave: true);
    }

    public async Task MarkAllAsReadAsync()
    {
        var tenantId = CurrentTenant.Id
            ?? throw new BusinessException(NotificationServiceErrorCodes.TenantNotFound);

        var userId = CurrentUser.GetId();

        var unreadNotifications = await _notificationRepository.GetUnreadListByUserAsync(tenantId, userId);

        if (unreadNotifications.Count == 0)
        {
            return;
        }
        
        var readAt = Clock.Now;

        foreach (var notification in unreadNotifications)
        {
            await _notificationManager.MarkAsReadAsync(notification, readAt);
        }

        await _notificationRepository.UpdateManyAsync(unreadNotifications, autoSave: true);
    }
    
    public async Task SendNotificationAsync(SendNotificationInput input)
    {
        Check.NotNull(input, nameof(input));
        if (input.RecipientUserId == Guid.Empty)
            throw new BusinessException(NotificationServiceErrorCodes.InvalidRecipient);

        if (input.TenantId == Guid.Empty)
            throw new BusinessException(NotificationServiceErrorCodes.TenantNotFound);
        
        var isDuplicate = await _notificationRepository.AnyAsync(
            n => n.TenantId == input.TenantId
                 && n.ReferenceId == input.EventId.ToString()
                 && n.ReferenceType == input.ReferenceType);

        if (isDuplicate)
        {
            return;
        }

        var notification = await _notificationManager.CreateAsync(
            tenantId:      input.TenantId,
            userId:        input.RecipientUserId,
            title:         input.Title,
            message:       input.Message,
            type:          input.Type,
            referenceType: input.ReferenceType,
            referenceId:   input.ReferenceId);

        await _notificationRepository.InsertAsync(notification, autoSave: true);
    }
}