using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class ReportingServiceApplicationTestBase<TStartupModule> : ReportingServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
