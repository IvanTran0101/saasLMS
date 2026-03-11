using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host;
using Volo.Saas.Tenant;

namespace saasLMS.SaasService.Web;

[DependsOn(
    typeof(SaasServiceApplicationContractsModule),
    typeof(SaasHostWebModule),
    typeof(SaasTenantWebModule)
)]
public class SaasServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SaasServiceWebModule>();
        });
        
        context.Services.AddMapperlyObjectMapper<SaasServiceWebModule>();
    }
}
