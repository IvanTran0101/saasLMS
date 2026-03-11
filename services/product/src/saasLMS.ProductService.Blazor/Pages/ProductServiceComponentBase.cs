using saasLMS.ProductService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.ProductService.Blazor.Pages;

public class ProductServiceComponentBase : AbpComponentBase
{
    public ProductServiceComponentBase()
    {
        LocalizationResource = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceBlazorModule);
    }
}
