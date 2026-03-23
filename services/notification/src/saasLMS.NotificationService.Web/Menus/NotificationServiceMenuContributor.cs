using System.Threading.Tasks;
using saasLMS.NotificationService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.NotificationService.Web.Menus;

public class NotificationServiceMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<NotificationServiceResource>();
        return Task.CompletedTask;
    }
}
