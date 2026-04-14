using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Dashboard;

[Authorize]
public partial class StudentDashboardPage : AbpComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        // Role guard: reads locally from claims — no remote call.
        if (!CurrentUser.IsInRole(LmsRoles.Student))
        {
            NavigationManager.NavigateTo("/");
        }

        return Task.CompletedTask;
    }
}
