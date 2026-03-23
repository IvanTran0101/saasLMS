using saasLMS.LearningProgressService.Localization;
using Volo.Abp.Application.Services;

namespace saasLMS.LearningProgressService;

public abstract class LearningProgressServiceAppService : ApplicationService
{
    protected LearningProgressServiceAppService()
    {
        LocalizationResource = typeof(LearningProgressServiceResource);
        ObjectMapperContext = typeof(LearningProgressServiceApplicationModule);
    }
}
