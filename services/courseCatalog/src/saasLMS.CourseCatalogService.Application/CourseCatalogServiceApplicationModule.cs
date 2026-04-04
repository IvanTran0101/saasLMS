using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Http.Client;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;


namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceDomainModule),
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpHttpClientModule)
    )]
public class CourseCatalogServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<CourseCatalogServiceApplicationModule>();
        context.Services.AddStaticHttpClientProxies(
            typeof(CourseCatalogServiceApplicationContractsModule).Assembly,
            "EnrollmentService");
    }
}
