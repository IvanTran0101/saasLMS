using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.Pro.Web;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.IdentityService.Web;

[DependsOn(
    typeof(AbpMapperlyModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpOpenIddictProWebModule),
    typeof(IdentityServiceApplicationContractsModule)
)]
public class IdentityServiceWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<IdentityServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<IdentityServiceWebModule>();
    }
}
