using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class LearningProgressServiceDomainTestBase<TStartupModule> : LearningProgressServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
