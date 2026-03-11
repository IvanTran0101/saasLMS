using Volo.Abp.Modularity;

namespace saasLMS.ProductService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class ProductServiceDomainTestBase<TStartupModule> : ProductServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
