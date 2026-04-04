using Microsoft.Extensions.DependencyInjection;
using saasLMS.EnrollmentService.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace saasLMS.EnrollmentService.Blazor;

[DependsOn(
    typeof(EnrollmentServiceApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpMapperlyModule)
    )]
public class EnrollmentServiceBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<EnrollmentServiceBlazorModule>();

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new EnrollmentServiceMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(EnrollmentServiceBlazorModule).Assembly);
        });
    }
}
