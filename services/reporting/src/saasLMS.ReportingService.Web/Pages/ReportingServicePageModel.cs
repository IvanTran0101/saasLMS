using saasLMS.ReportingService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.ReportingService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class ReportingServicePageModel : AbpPageModel
{
    protected ReportingServicePageModel()
    {
        LocalizationResourceType = typeof(ReportingServiceResource);
        ObjectMapperContext = typeof(ReportingServiceWebModule);
    }
}
