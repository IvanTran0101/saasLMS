using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.LearningProgressService.Localization;
using saasLMS.LearningProgressService.Permissions;
using saasLMS.LearningProgressService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.LearningProgressService.Web;

[DependsOn(
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class LearningProgressServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(LearningProgressServiceResource), typeof(LearningProgressServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(LearningProgressServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new LearningProgressServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<LearningProgressServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<LearningProgressServiceWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
                // options.Conventions.AuthorizePage("/LearningProgressService/Index", LearningProgressServicePermissions.LearningProgressService.Default);
            });
    }
}
