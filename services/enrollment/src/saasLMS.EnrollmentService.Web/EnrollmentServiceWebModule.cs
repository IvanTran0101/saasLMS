using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.EnrollmentService.Localization;
using saasLMS.EnrollmentService.Permissions;
using saasLMS.EnrollmentService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.EnrollmentService.Web;

[DependsOn(
    typeof(EnrollmentServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class EnrollmentServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(EnrollmentServiceResource), typeof(EnrollmentServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(EnrollmentServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new EnrollmentServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<EnrollmentServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<EnrollmentServiceWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
                // options.Conventions.AuthorizePage("/EnrollmentService/Index", EnrollmentServicePermissions.EnrollmentService.Default);
            });
    }
}
