using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class AssessmentServiceApplicationTestBase<TStartupModule> : AssessmentServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
