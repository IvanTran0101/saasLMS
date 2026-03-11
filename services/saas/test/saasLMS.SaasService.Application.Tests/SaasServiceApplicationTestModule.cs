using saasLMS.SaasService.Application;
using Volo.Abp.Modularity;

namespace saasLMS.SaasService;

[DependsOn(
    typeof(SaasServiceApplicationModule),
    typeof(SaasServiceDomainTestModule)
    )]
public class SaasServiceApplicationTestModule : AbpModule
{

}
