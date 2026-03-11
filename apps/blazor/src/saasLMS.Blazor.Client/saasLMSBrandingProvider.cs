using Microsoft.Extensions.Localization;
using saasLMS.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace saasLMS.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class saasLMSBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<saasLMSResource> _localizer;

    public saasLMSBrandingProvider(IStringLocalizer<saasLMSResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
