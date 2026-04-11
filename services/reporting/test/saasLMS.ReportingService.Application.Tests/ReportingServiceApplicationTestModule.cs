using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

[DependsOn(
    typeof(ReportingServiceApplicationModule),
    typeof(ReportingServiceDomainTestModule)
    )]
public class ReportingServiceApplicationTestModule : AbpModule
{

}
