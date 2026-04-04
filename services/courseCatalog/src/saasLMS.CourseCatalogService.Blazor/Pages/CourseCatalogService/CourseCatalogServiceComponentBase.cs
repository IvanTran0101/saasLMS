using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.CourseCatalogService.Blazor.Pages.CourseCatalogService;

public class CourseCatalogServiceComponentBase : AbpComponentBase
{
    public CourseCatalogServiceComponentBase()
    {
        LocalizationResource = typeof(CourseCatalogServiceResource);
        ObjectMapperContext = typeof(CourseCatalogServiceBlazorModule);
    }
}
