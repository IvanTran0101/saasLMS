using saasLMS.ReportingService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.ReportingService;

public abstract class ReportingServiceAppService : ApplicationService
{
    protected ReportingServiceAppService()
    {
        LocalizationResource = typeof(ReportingServiceResource);
        ObjectMapperContext = typeof(ReportingServiceApplicationModule);
    }
}
