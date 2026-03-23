using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class LearningProgressServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(LearningProgressServiceApplicationContractsModule).Assembly,
            LearningProgressServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<LearningProgressServiceHttpApiClientModule>();
        });
    }
}
