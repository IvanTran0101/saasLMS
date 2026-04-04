using saasLMS.NotificationService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.NotificationService;

public abstract class NotificationServiceAppService : ApplicationService
{
    protected NotificationServiceAppService()
    {
        LocalizationResource = typeof(NotificationServiceResource);
        ObjectMapperContext = typeof(NotificationServiceApplicationModule);
    }
}
