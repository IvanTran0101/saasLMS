using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.NotificationService.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(NotificationServiceApplicationModule),
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class NotificationServiceHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(NotificationServiceHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(
                typeof(NotificationServiceApplicationModule).Assembly,
                opts => { opts.RootPath = "notification"; });
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<NotificationServiceResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
