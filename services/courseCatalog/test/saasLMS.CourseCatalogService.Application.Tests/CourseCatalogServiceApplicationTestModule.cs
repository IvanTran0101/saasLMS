using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceApplicationModule),
    typeof(CourseCatalogServiceDomainTestModule)
    )]
public class CourseCatalogServiceApplicationTestModule : AbpModule
{

}
