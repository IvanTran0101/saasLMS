using System.Threading.Tasks;
using saasLMS.AssessmentService.Localization;
using Volo.Abp.UI.Navigation;

namespace saasLMS.AssessmentService.Web.Menus;

public class AssessmentServiceMenuContributor : IMenuContributor
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
        var l = context.GetLocalizer<AssessmentServiceResource>();
        return Task.CompletedTask;
    }
}
