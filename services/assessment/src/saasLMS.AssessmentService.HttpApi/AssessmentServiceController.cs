using saasLMS.AssessmentService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.AssessmentService;

public abstract class AssessmentServiceController : AbpControllerBase
{
    protected AssessmentServiceController()
    {
        LocalizationResource = typeof(AssessmentServiceResource);
    }
}
