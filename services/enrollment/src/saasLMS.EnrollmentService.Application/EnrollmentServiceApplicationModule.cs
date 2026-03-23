using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(EnrollmentServiceDomainModule),
    typeof(EnrollmentServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class EnrollmentServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<EnrollmentServiceApplicationModule>();
    }
}
