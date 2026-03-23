using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(EnrollmentServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class EnrollmentServiceApplicationContractsModule : AbpModule
{

}
