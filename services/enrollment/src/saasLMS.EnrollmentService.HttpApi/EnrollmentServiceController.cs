using saasLMS.EnrollmentService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.EnrollmentService;

public abstract class EnrollmentServiceController : AbpControllerBase
{
    protected EnrollmentServiceController()
    {
        LocalizationResource = typeof(EnrollmentServiceResource);
    }
}
