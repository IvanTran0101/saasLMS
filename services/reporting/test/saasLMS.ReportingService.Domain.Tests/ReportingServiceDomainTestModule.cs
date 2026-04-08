using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(ReportingServiceDomainModule),
    typeof(ReportingServiceTestBaseModule)
)]
public class ReportingServiceDomainTestModule : AbpModule
{

}
