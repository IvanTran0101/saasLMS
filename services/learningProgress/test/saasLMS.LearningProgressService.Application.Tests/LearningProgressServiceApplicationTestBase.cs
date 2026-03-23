using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class LearningProgressServiceApplicationTestBase<TStartupModule> : LearningProgressServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
