using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(LearningProgressServiceDomainSharedModule)
)]
public class LearningProgressServiceDomainModule : AbpModule
{
}
