using Microsoft.Extensions.DependencyInjection;
using saasLMS.CourseCatalogService.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace saasLMS.CourseCatalogService.Blazor;

[DependsOn(
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpMapperlyModule)
    )]
public class CourseCatalogServiceBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<CourseCatalogServiceBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CourseCatalogServiceMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(CourseCatalogServiceBlazorModule).Assembly);
        });
    }
}
