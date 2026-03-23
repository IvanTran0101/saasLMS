using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(LearningProgressServiceDomainModule),
    typeof(LearningProgressServiceTestBaseModule)
)]
public class LearningProgressServiceDomainTestModule : AbpModule
{

}
