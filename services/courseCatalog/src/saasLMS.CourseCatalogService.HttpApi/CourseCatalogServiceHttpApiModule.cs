using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc.Conventions;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(CourseCatalogServiceApplicationModule),
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CourseCatalogServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CourseCatalogServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(
                typeof(CourseCatalogServiceApplicationModule).Assembly,
                opts =>
                {
                    opts.RootPath = "course-catalog";
                    opts.RemoteServiceName = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName;
                });
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CourseCatalogServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
