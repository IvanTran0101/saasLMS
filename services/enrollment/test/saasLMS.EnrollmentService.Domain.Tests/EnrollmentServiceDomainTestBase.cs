using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class EnrollmentServiceDomainTestBase<TStartupModule> : EnrollmentServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
