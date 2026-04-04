using saasLMS.EnrollmentService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.EnrollmentService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class EnrollmentServicePageModel : AbpPageModel
{
    protected EnrollmentServicePageModel()
    {
        LocalizationResourceType = typeof(EnrollmentServiceResource);
        ObjectMapperContext = typeof(EnrollmentServiceWebModule);
    }
}
