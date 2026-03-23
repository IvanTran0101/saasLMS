using saasLMS.NotificationService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.NotificationService.Blazor.Pages.NotificationService;

public class NotificationServiceComponentBase : AbpComponentBase
{
    public NotificationServiceComponentBase()
    {
        LocalizationResource = typeof(NotificationServiceResource);
        ObjectMapperContext = typeof(NotificationServiceBlazorModule);
    }
}
