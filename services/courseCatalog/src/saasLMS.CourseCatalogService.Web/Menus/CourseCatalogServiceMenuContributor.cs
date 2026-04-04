using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.CourseCatalogService.Web.Menus;

public class CourseCatalogServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<CourseCatalogServiceResource>();
        return Task.CompletedTask;
    }
}
