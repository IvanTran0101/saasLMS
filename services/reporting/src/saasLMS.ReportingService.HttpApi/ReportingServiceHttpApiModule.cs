using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.ReportingService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(ReportingServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class ReportingServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ReportingServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ReportingServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
