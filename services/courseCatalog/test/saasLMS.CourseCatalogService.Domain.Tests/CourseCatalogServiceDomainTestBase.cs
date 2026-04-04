using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class CourseCatalogServiceDomainTestBase<TStartupModule> : CourseCatalogServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
