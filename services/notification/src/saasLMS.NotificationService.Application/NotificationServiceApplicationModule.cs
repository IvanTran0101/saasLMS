using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Emailing;
using Volo.Abp.MailKit;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpEmailingModule),
    typeof(AbpMailKitModule)
    )]
public class NotificationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<NotificationServiceApplicationModule>();
    }
}
