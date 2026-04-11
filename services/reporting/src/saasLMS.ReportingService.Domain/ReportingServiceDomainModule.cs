using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(ReportingServiceDomainSharedModule)
)]
public class ReportingServiceDomainModule : AbpModule
{
}
