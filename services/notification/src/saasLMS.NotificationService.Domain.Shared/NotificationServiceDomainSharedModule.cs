using saasLMS.NotificationService.Localization;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.NotificationService;

[DependsOn(
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpValidationModule)
)]
public class NotificationServiceDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        NotificationServiceModuleExtensionConfigurator.Configure();
        NotificationServiceGlobalFeatureConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NotificationServiceDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<NotificationServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/NotificationService");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("NotificationService", typeof(NotificationServiceResource));
        });
    }
}
