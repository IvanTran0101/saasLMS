using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(NotificationServiceDomainSharedModule)
)]
public class NotificationServiceDomainModule : AbpModule
{
}
