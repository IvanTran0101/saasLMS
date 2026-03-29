using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.CourseCatalogService;

public abstract class CourseCatalogServiceAppService : ApplicationService
{
    protected CourseCatalogServiceAppService()
    {
        LocalizationResource = typeof(CourseCatalogServiceResource);
        ObjectMapperContext = typeof(CourseCatalogServiceApplicationModule);
    }
}
