using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class CourseCatalogServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(CourseCatalogServiceApplicationContractsModule).Assembly,
            CourseCatalogServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CourseCatalogServiceHttpApiClientModule>();
        });
    }
}
