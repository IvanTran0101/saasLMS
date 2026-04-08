using saasLMS.ReportingService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.ReportingService.Blazor.Pages.ReportingService;

public class ReportingServiceComponentBase : AbpComponentBase
{
    public ReportingServiceComponentBase()
    {
        LocalizationResource = typeof(ReportingServiceResource);
        ObjectMapperContext = typeof(ReportingServiceBlazorModule);
    }
}
