using saasLMS.ProductService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.ProductService;

public abstract class ProductServiceController : AbpController
{
    protected ProductServiceController()
    {
        LocalizationResource = typeof(ProductServiceResource);
    }
}
