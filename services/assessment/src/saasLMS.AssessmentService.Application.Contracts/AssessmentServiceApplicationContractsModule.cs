using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AssessmentServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class AssessmentServiceApplicationContractsModule : AbpModule
{

}
