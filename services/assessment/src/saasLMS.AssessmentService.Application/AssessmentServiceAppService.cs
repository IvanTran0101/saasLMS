using saasLMS.AssessmentService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.AssessmentService;

public abstract class AssessmentServiceAppService : ApplicationService
{
    protected AssessmentServiceAppService()
    {
        LocalizationResource = typeof(AssessmentServiceResource);
        ObjectMapperContext = typeof(AssessmentServiceApplicationModule);
    }
}
