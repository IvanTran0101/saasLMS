using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace saasLMS.Blazor.Client;

public class saasLMSBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        // Keep this contributor intentionally empty.
        // The Blazor host page links `global.css`, `app-shell.css`, and `main.css`
        // directly as static web assets. Re-registering `main.css` here creates an
        // ambiguous route in the ABP bundling pipeline and can cause the browser to
        // receive a different asset than the source file we expect.
    }
}
