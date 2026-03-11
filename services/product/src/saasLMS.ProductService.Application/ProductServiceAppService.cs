using saasLMS.ProductService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.ProductService;

public abstract class ProductServiceAppService : ApplicationService
{
    protected ProductServiceAppService()
    {
        LocalizationResource = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceApplicationModule);
    }
}
