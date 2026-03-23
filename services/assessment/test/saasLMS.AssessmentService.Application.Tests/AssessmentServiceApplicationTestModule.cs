using Volo.Abp.Modularity;

namespace saasLMS.AssessmentService;

[DependsOn(
    typeof(AssessmentServiceApplicationModule),
    typeof(AssessmentServiceDomainTestModule)
    )]
public class AssessmentServiceApplicationTestModule : AbpModule
{

}
