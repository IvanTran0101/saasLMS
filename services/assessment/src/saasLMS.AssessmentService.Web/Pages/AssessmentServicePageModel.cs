using saasLMS.AssessmentService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.AssessmentService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class AssessmentServicePageModel : AbpPageModel
{
    protected AssessmentServicePageModel()
    {
        LocalizationResourceType = typeof(AssessmentServiceResource);
        ObjectMapperContext = typeof(AssessmentServiceWebModule);
    }
}
