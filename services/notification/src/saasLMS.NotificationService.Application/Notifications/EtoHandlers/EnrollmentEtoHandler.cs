using System.Threading.Tasks;
using saasLMS.NotificationService.Etos.Enrollments;
using saasLMS.NotificationService.Notifications.Dtos.Inputs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.NotificationService.Notifications.EtoHandlers;

public class EnrollmentEtoHandler
    : IDistributedEventHandler<StudentEnrolledEto>,
        ITransientDependency
{
    private readonly INotificationAppService _notificationAppService;

    public EnrollmentEtoHandler(INotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    public async Task HandleEventAsync(StudentEnrolledEto eventData)
    {
        await _notificationAppService.SendNotificationAsync(new SendNotificationInput
        {
            // EventId từ ETO — AppService dùng để idempotency check
            EventId         = eventData.EventId,
            TenantId        = eventData.TenantId,
            RecipientUserId = eventData.StudentId,
            Title           = "Đăng ký khóa học thành công",
            Message         = "Bạn đã đăng ký khóa học thành công. Chúc bạn học tốt!",
            Type            = NotificationType.Enrollment,
            ReferenceType   = nameof(StudentEnrolledEto),
            ReferenceId     = eventData.EnrollmentId.ToString()
        });
    }
}