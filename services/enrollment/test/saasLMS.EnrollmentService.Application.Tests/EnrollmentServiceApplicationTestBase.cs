using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class EnrollmentServiceApplicationTestBase<TStartupModule> : EnrollmentServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
