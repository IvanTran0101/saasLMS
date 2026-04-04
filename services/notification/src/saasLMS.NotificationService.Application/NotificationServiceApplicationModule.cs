using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class NotificationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<NotificationServiceApplicationModule>();
    }
}
