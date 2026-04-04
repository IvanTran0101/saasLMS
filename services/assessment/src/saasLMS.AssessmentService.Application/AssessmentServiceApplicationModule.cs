using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AssessmentServiceDomainModule),
    typeof(AssessmentServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class AssessmentServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<AssessmentServiceApplicationModule>();
    }
}
