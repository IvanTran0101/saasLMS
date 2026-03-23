using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceApplicationModule),
    typeof(NotificationServiceDomainTestModule)
    )]
public class NotificationServiceApplicationTestModule : AbpModule
{

}
