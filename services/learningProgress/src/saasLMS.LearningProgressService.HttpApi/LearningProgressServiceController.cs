using saasLMS.LearningProgressService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.LearningProgressService;

public abstract class LearningProgressServiceController : AbpControllerBase
{
    protected LearningProgressServiceController()
    {
        LocalizationResource = typeof(LearningProgressServiceResource);
    }
}
