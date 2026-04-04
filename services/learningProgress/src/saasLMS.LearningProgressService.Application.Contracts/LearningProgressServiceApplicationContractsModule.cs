using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class LearningProgressServiceApplicationContractsModule : AbpModule
{

}
