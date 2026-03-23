using saasLMS.NotificationService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.NotificationService;

public abstract class NotificationServiceController : AbpControllerBase
{
    protected NotificationServiceController()
    {
        LocalizationResource = typeof(NotificationServiceResource);
    }
}
