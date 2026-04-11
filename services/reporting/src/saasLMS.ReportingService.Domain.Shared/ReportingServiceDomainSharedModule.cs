using saasLMS.ReportingService.Localization;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpValidationModule)
)]
public class ReportingServiceDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ReportingServiceModuleExtensionConfigurator.Configure();
        ReportingServiceGlobalFeatureConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ReportingServiceDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<ReportingServiceResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/ReportingService");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("ReportingService", typeof(ReportingServiceResource));
        });
    }
}
