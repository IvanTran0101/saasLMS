using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(EnrollmentServiceApplicationModule),
    typeof(EnrollmentServiceDomainTestModule)
    )]
public class EnrollmentServiceApplicationTestModule : AbpModule
{

}
