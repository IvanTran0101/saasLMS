using Microsoft.Extensions.DependencyInjection;
using saasLMS.LearningProgressService.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace saasLMS.LearningProgressService.Blazor;

[DependsOn(
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpMapperlyModule)
    )]
public class LearningProgressServiceBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<LearningProgressServiceBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new LearningProgressServiceMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(LearningProgressServiceBlazorModule).Assembly);
        });
    }
}
