using Volo.Abp.Modularity;

namespace saasLMS.NotificationService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class NotificationServiceApplicationTestBase<TStartupModule> : NotificationServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
