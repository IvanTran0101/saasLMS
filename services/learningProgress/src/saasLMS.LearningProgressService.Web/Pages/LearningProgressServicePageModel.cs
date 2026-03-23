using saasLMS.LearningProgressService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.LearningProgressService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class LearningProgressServicePageModel : AbpPageModel
{
    protected LearningProgressServicePageModel()
    {
        LocalizationResourceType = typeof(LearningProgressServiceResource);
        ObjectMapperContext = typeof(LearningProgressServiceWebModule);
    }
}
