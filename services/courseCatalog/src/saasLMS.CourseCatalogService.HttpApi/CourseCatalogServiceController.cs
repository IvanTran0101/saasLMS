using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.CourseCatalogService;

public abstract class CourseCatalogServiceController : AbpControllerBase
{
    protected CourseCatalogServiceController()
    {
        LocalizationResource = typeof(CourseCatalogServiceResource);
    }
}
