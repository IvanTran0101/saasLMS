using saasLMS.AdministrationService;
using saasLMS.AdministrationService.EntityFrameworkCore;
using saasLMS.IdentityService;
using saasLMS.IdentityService.EntityFrameworkCore;
using saasLMS.ProductService;
using saasLMS.ProductService.EntityFrameworkCore;
using saasLMS.SaasService;
using saasLMS.SaasService.EntityFrameworkCore;
using saasLMS.Shared.Hosting;
using Volo.Abp.Modularity;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;

namespace saasLMS.DbMigrator;

[DependsOn(
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(saasLMSSharedHostingModule),
    typeof(IdentityServiceEntityFrameworkCoreModule),
    typeof(IdentityServiceApplicationContractsModule),
    typeof(SaasServiceEntityFrameworkCoreModule),
    typeof(SaasServiceApplicationContractsModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceApplicationContractsModule),
    typeof(ProductServiceApplicationContractsModule),
    typeof(ProductServiceEntityFrameworkCoreModule)
)]
public class saasLMSDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "saasLMS:"; });
    }
}
