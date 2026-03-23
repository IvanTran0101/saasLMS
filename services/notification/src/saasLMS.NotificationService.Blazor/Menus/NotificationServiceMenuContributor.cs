using System.Threading.Tasks;
using saasLMS.NotificationService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.NotificationService.Blazor.Menus;

public class NotificationServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<NotificationServiceResource>();
        return Task.CompletedTask;
    }
}
