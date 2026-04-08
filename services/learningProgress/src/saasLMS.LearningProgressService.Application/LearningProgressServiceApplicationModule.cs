using Microsoft.Extensions.DependencyInjection;
using saasLMS.EnrollmentService;
using Volo.Abp.Application;
using Volo.Abp.EventBus;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Http.Client;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceDomainModule),
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpEventBusModule),
    typeof(AbpHttpClientModule)
    )]
public class LearningProgressServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<LearningProgressServiceApplicationModule>();
        
        context.Services.AddStaticHttpClientProxies(
            typeof(EnrollmentServiceApplicationContractsModule).Assembly,
            remoteServiceConfigurationName: "EnrollmentService");
    }
}
