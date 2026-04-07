using Microsoft.Extensions.DependencyInjection;
using saasLMS.CourseCatalogService;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AssessmentServiceApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AssessmentServiceHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddStaticHttpClientProxies(typeof(AssessmentServiceApplicationContractsModule).Assembly,
            AssessmentServiceRemoteServiceConsts.RemoteServiceName);
        context.Services.AddStaticHttpClientProxies(
            typeof(CourseCatalogServiceApplicationContractsModule).Assembly,
            "course-catalog");

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AssessmentServiceHttpApiClientModule>();
        });
    }
}
