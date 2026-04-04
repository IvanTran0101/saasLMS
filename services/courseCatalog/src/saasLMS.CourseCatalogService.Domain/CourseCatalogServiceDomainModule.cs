using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(CourseCatalogServiceDomainSharedModule)
)]
public class CourseCatalogServiceDomainModule : AbpModule
{
}
