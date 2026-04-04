using saasLMS.EnrollmentService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.EnrollmentService.Blazor.Pages.EnrollmentService;

public class EnrollmentServiceComponentBase : AbpComponentBase
{
    public EnrollmentServiceComponentBase()
    {
        LocalizationResource = typeof(EnrollmentServiceResource);
        ObjectMapperContext = typeof(EnrollmentServiceBlazorModule);
    }
}
