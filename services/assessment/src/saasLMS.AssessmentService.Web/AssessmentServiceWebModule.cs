using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.AssessmentService.Localization;
using saasLMS.AssessmentService.Permissions;
using saasLMS.AssessmentService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.AssessmentService.Web;

[DependsOn(
    typeof(AssessmentServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class AssessmentServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(AssessmentServiceResource), typeof(AssessmentServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AssessmentServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AssessmentServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AssessmentServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<AssessmentServiceWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
                // options.Conventions.AuthorizePage("/AssessmentService/Index", AssessmentServicePermissions.AssessmentService.Default);
            });
    }
}
