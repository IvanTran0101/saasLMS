using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.NotificationService.Notifications;

public class Notification : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid UserId { get; protected set; }
    public string Title { get; protected set; }
    public string Message { get; protected set; }
    public NotificationType Type { get; protected set; }
    public bool IsRead { get; protected set; }
    public DateTime? ReadAt { get; protected set; }
    public string? ReferenceType { get; protected set; }
    public string? ReferenceId { get; protected set; }
    protected Notification() { }

    public Notification(Guid id, Guid tenantId, Guid userId, string title, string message, NotificationType type, string? referenceType = null, string? referenceId = null) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("The user id cannot be empty.", nameof(userId));
            
        }
        
        TenantId = tenantId;
        UserId = userId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Message = Check.NotNullOrWhiteSpace(message, nameof(message));
        Type = type;
        IsRead = false;
        ReadAt = null;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
    }
    public void MarkAsRead(DateTime readAt)
    {
        if (IsRead)
        {
            return;
        }
        IsRead = true;
        ReadAt = readAt;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }
    
}