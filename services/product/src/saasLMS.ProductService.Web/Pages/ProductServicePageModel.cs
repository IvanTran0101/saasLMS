using saasLMS.ProductService.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace saasLMS.ProductService.Web.Pages;

/* Inherit your PageModel classes from this class. */
public abstract class ProductServicePageModel : AbpPageModel
{
    protected ProductServicePageModel()
    {
        LocalizationResourceType = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceWebModule);
    }
}
