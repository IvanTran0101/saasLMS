using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn;

[Authorize]
public partial class LessonViewerPage : AbpComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {

        return Task.CompletedTask;
    }
}
