using saasLMS.EnrollmentService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.EnrollmentService;

public abstract class EnrollmentServiceAppService : ApplicationService
{
    protected EnrollmentServiceAppService()
    {
        LocalizationResource = typeof(EnrollmentServiceResource);
        ObjectMapperContext = typeof(EnrollmentServiceApplicationModule);
    }
}
