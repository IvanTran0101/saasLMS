using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(EnrollmentServiceDomainModule),
    typeof(EnrollmentServiceTestBaseModule)
)]
public class EnrollmentServiceDomainTestModule : AbpModule
{

}
