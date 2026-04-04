using Microsoft.Extensions.DependencyInjection;
using saasLMS.NotificationService.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace saasLMS.NotificationService.Blazor;

[DependsOn(
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpMapperlyModule)
    )]
public class NotificationServiceBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<NotificationServiceBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new NotificationServiceMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(NotificationServiceBlazorModule).Assembly);
        });
    }
}
