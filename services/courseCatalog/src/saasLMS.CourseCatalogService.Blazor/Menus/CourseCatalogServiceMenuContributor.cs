using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.CourseCatalogService.Blazor.Menus;

public class CourseCatalogServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<CourseCatalogServiceResource>();
        return Task.CompletedTask;
    }
}
