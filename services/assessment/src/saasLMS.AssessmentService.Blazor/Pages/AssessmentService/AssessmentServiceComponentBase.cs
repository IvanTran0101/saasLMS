using saasLMS.AssessmentService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.AssessmentService.Blazor.Pages.AssessmentService;

public class AssessmentServiceComponentBase : AbpComponentBase
{
    public AssessmentServiceComponentBase()
    {
        LocalizationResource = typeof(AssessmentServiceResource);
        ObjectMapperContext = typeof(AssessmentServiceBlazorModule);
    }
}
