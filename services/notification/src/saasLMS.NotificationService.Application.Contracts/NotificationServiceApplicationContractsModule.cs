using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class NotificationServiceApplicationContractsModule : AbpModule
{

}
