using Volo.Abp.Modularity;

namespace saasLMS.ProductService;

[DependsOn(
    typeof(ProductServiceApplicationModule),
    typeof(ProductServiceDomainTestModule)
    )]
public class ProductServiceApplicationTestModule : AbpModule
{

}
