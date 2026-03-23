using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

[DependsOn(
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
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CourseCatalogServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
