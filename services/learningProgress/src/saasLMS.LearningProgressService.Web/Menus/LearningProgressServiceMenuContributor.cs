using System.Threading.Tasks;
using saasLMS.LearningProgressService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.LearningProgressService.Web.Menus;

public class LearningProgressServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<LearningProgressServiceResource>();
        return Task.CompletedTask;
    }
}
