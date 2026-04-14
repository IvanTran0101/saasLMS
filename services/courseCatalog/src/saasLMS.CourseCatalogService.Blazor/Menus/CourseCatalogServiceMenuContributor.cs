using System.Threading.Tasks;
using Volo.Abp.UI.Navigation;

namespace saasLMS.CourseCatalogService.Blazor.Menus;

public class CourseCatalogServiceMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        // Menu items are managed centrally in saasLMSMenuContributor (BlazorClient).
        return Task.CompletedTask;
    }
}
