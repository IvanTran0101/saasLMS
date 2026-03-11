using Microsoft.Extensions.DependencyInjection;
using saasLMS.Shared.Hosting.AspNetCore;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace saasLMS.Shared.Hosting.Gateways;

[DependsOn(
    typeof(saasLMSSharedHostingAspNetCoreModule),    
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpSwashbuckleModule)
)]
public class saasLMSSharedHostingGatewaysModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var env = context.Services.GetHostingEnvironment();

        context.Services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    }
}
