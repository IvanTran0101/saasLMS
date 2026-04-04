using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(EnrollmentServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class EnrollmentServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(EnrollmentServiceApplicationContractsModule).Assembly,
            EnrollmentServiceRemoteServiceConsts.RemoteServiceName);

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<EnrollmentServiceHttpApiClientModule>();
        });
    }
}
