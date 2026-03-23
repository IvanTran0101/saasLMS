using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.AssessmentService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AssessmentServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class AssessmentServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AssessmentServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AssessmentServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
