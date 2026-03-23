using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceDomainModule),
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class CourseCatalogServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<CourseCatalogServiceApplicationModule>();
    }
}
