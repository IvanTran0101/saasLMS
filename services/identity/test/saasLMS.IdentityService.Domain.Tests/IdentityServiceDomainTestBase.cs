using Volo.Abp.Modularity;

namespace saasLMS.IdentityService;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class IdentityServiceDomainTestBase<TStartupModule> : IdentityServiceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
