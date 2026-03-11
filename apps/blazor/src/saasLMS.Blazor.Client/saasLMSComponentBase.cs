using saasLMS.Localization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client;

public abstract class saasLMSComponentBase : AbpComponentBase
{
    protected saasLMSComponentBase()
    {
        LocalizationResource = typeof(saasLMSResource);
    }
}
