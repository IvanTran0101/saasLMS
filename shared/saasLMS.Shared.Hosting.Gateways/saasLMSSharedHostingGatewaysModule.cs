using Microsoft.Extensions.DependencyInjection;
using saasLMS.Shared.Hosting.AspNetCore;
using saasLMS.Shared.Hosting.Gateways.ReverseProxy;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

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

        // Strip browser CORS headers before proxying to downstream services.
        context.Services.AddSingleton<ITransformProvider, StripOriginTransformProvider>();

        context.Services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
    }
}
