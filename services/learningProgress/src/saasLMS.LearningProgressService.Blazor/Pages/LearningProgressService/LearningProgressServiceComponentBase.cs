using saasLMS.LearningProgressService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.LearningProgressService.Blazor.Pages.LearningProgressService;

public class LearningProgressServiceComponentBase : AbpComponentBase
{
    public LearningProgressServiceComponentBase()
    {
        LocalizationResource = typeof(LearningProgressServiceResource);
        ObjectMapperContext = typeof(LearningProgressServiceBlazorModule);
    }
}
