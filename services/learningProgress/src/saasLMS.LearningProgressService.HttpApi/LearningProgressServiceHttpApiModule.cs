using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.LearningProgressService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class LearningProgressServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(LearningProgressServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<LearningProgressServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
