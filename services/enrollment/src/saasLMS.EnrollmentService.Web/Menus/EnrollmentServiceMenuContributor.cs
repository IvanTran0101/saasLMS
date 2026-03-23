using System.Threading.Tasks;
using saasLMS.EnrollmentService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.EnrollmentService.Web.Menus;

public class EnrollmentServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<EnrollmentServiceResource>();
        return Task.CompletedTask;
    }
}
