using System.Threading.Tasks;
using saasLMS.ReportingService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.ReportingService.Blazor.Menus;

public class ReportingServiceMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ReportingServiceResource>();
        return Task.CompletedTask;
    }
}
