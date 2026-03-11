using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace saasLMS.IdentityService;

[DependsOn(
    typeof(AbpMapperlyModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpOpenIddictProApplicationModule),
    typeof(IdentityServiceDomainModule),
    typeof(AbpAccountAdminApplicationModule),
    typeof(IdentityServiceApplicationContractsModule)
)]
public class IdentityServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<IdentityServiceApplicationModule>();
    }
}
