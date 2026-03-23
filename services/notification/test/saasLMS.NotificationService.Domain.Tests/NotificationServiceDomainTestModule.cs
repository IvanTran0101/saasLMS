using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(NotificationServiceTestBaseModule)
)]
public class NotificationServiceDomainTestModule : AbpModule
{

}
