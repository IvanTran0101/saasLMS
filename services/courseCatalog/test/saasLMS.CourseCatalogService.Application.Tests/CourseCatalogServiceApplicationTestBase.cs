using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class CourseCatalogServiceApplicationTestBase<TStartupModule> : CourseCatalogServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
