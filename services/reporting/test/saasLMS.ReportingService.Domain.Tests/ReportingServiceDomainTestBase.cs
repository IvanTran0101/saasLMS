using Volo.Abp.Modularity;

namespace saasLMS.ReportingService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class ReportingServiceDomainTestBase<TStartupModule> : ReportingServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
