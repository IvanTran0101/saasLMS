using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace saasLMS.SaasService.Application;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(SaasServiceDomainModule),
    typeof(SaasTenantApplicationModule),
    typeof(SaasHostApplicationModule)
)]
public class SaasServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<SaasServiceApplicationModule>();
    }
}
