using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class CourseCatalogServiceApplicationContractsModule : AbpModule
{

}
