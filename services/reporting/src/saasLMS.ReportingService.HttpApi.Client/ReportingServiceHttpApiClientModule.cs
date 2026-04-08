using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(ReportingServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class ReportingServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(ReportingServiceApplicationContractsModule).Assembly,
            ReportingServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ReportingServiceHttpApiClientModule>();
        });
    }
}
