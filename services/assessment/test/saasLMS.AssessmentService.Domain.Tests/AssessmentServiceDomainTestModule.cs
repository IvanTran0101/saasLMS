using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AssessmentServiceDomainModule),
    typeof(AssessmentServiceTestBaseModule)
)]
public class AssessmentServiceDomainTestModule : AbpModule
{

}
