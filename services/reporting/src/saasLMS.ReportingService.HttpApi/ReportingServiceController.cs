using saasLMS.ReportingService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.ReportingService;

public abstract class ReportingServiceController : AbpControllerBase
{
    protected ReportingServiceController()
    {
        LocalizationResource = typeof(ReportingServiceResource);
    }
}
