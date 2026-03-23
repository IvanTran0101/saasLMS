using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.CourseCatalogService.Localization;
using saasLMS.CourseCatalogService.Permissions;
using saasLMS.CourseCatalogService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.CourseCatalogService.Web;

[DependsOn(
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class CourseCatalogServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(CourseCatalogServiceResource), typeof(CourseCatalogServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CourseCatalogServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CourseCatalogServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CourseCatalogServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<CourseCatalogServiceWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
                // options.Conventions.AuthorizePage("/CourseCatalogService/Index", CourseCatalogServicePermissions.CourseCatalogService.Default);
            });
    }
}
