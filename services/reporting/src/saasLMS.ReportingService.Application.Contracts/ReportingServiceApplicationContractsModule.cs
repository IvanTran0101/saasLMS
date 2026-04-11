using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(ReportingServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class ReportingServiceApplicationContractsModule : AbpModule
{

}
