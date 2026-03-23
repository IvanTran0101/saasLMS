using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class NotificationServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(NotificationServiceApplicationContractsModule).Assembly,
            NotificationServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NotificationServiceHttpApiClientModule>();
        });
    }
}
