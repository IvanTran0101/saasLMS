using Volo.Abp.Modularity;
using Volo.Saas;

namespace saasLMS.SaasService;

[DependsOn(
    typeof(SaasServiceDomainSharedModule),
    typeof(SaasDomainModule)
)]
public class SaasServiceDomainModule : AbpModule
{
}
