using Volo.Abp.Modularity;

namespace saasLMS.CourseCatalogService;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(CourseCatalogServiceDomainModule),
    typeof(CourseCatalogServiceTestBaseModule)
)]
public class CourseCatalogServiceDomainTestModule : AbpModule
{

}
