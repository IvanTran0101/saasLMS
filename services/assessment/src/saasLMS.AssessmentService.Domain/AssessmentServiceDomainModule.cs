using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(AssessmentServiceDomainSharedModule)
)]
public class AssessmentServiceDomainModule : AbpModule
{
}
