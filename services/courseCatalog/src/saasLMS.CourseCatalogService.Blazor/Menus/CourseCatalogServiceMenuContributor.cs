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
        
        context.Menu.AddItem(new ApplicationMenuItem(
            CourseCatalogServiceMenus.Dashboard,
            l["Menu:Dashboard"],
            "/instructor/dashboard",
            icon: "fa fa-grip-horizontal",
            order: 1
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            CourseCatalogServiceMenus.Assignment,
            l["Menu:Assignment"],
            "/courses",
            icon: "fa fa-book-open",
            order: 2
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            CourseCatalogServiceMenus.Students,
            l["Menu:Students"],
            "/students",
            icon: "fa fa-users",
            order: 3
        ));
        
        return Task.CompletedTask;
    }
}
