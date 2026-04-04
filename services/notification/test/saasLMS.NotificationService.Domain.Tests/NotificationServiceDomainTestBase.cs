using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class NotificationServiceDomainTestBase<TStartupModule> : NotificationServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
