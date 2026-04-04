using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.EventBus;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceDomainModule),
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpEventBusModule)
    )]
public class LearningProgressServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<LearningProgressServiceApplicationModule>();
    }
}
