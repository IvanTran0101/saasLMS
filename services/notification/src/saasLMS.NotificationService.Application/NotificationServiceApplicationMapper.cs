using Riok.Mapperly.Abstractions;
using saasLMS.NotificationService.Notifications;
using saasLMS.NotificationService.Notifications.Dtos.Outputs;
using Volo.Abp.Mapperly;

namespace saasLMS.NotificationService;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class NotificationToNotificationDtoMapper : MapperBase<Notification, NotificationDto>
{
    public override partial NotificationDto Map(Notification source);
    public override partial void Map(Notification source, NotificationDto destination);
}