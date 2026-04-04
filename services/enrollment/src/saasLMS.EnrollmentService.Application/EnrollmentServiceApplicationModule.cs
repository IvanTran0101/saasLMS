using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.EventBus;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(EnrollmentServiceDomainModule),
    typeof(EnrollmentServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpEventBusModule)
    )]
public class EnrollmentServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<EnrollmentServiceApplicationModule>();
    }
}
