using Volo.Abp.Domain;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(EnrollmentServiceDomainSharedModule)
)]
public class EnrollmentServiceDomainModule : AbpModule
{
}
