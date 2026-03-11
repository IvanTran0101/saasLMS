using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace saasLMS.Blazor.Client;

public class saasLMSBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add(new BundleFile("main.css", true));
    }
}

