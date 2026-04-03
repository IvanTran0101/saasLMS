using System.Threading.Tasks;
using saasLMS.NotificationService.Notifications.Dtos.Inputs;
using Volo.Abp.DependencyInjection;

namespace saasLMS.NotificationService.Notifications.EtoHandlers;

public class AssessmentEtoHandler : ITransientDependency
{
    private readonly INotificationAppService _notificationAppService;

    public AssessmentEtoHandler(INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }
    
    public async Task HandleEventAsync(SubmissionGradedEto eventData)
    {
        await _notificationAppService.SendNotificationAsync(new SendNotificationInput
        {
            EventId         = eventData.EventId,
            TenantId        = eventData.TenantId,
            RecipientUserId = eventData.StudentId,
            Title           = "Bài tập của bạn đã được chấm điểm",
            Message         = $"Bạn đạt {eventData.Score}/{eventData.MaxScore} điểm.",
            Type            = NotificationType.Assignment,
            ReferenceType   = nameof(SubmissionGradedEto),
            ReferenceId     = eventData.SubmissionId.ToString()
        });
    }
}