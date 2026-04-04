using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService;

[DependsOn(
    typeof(LearningProgressServiceApplicationModule),
    typeof(LearningProgressServiceDomainTestModule)
    )]
public class LearningProgressServiceApplicationTestModule : AbpModule
{

}
