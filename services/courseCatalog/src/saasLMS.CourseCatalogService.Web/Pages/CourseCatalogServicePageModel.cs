using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.CourseCatalogService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class CourseCatalogServicePageModel : AbpPageModel
{
    protected CourseCatalogServicePageModel()
    {
        LocalizationResourceType = typeof(CourseCatalogServiceResource);
        ObjectMapperContext = typeof(CourseCatalogServiceWebModule);
    }
}
