using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.ReportingService.Reports;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Admin.Dashboard;

[Authorize(Roles = LmsRoles.Admin)]
public partial class AdminDashboardPage : AbpComponentBase
{
    [Inject]
    private IReportingAppService ReportingAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private bool _isLoading = true;
    private TenantSummaryReportViewDto? _summary;

    // ── SVG donut shared circumference (r=40) ───────────────────────────────
    private static double DonutCirc => 2.0 * Math.PI * 40.0;
    private static readonly System.Globalization.CultureInfo _inv =
        System.Globalization.CultureInfo.InvariantCulture;

    // ── Students donut ───────────────────────────────────────────────────────
    private double StDenom      => Math.Max(1.0, (double)(_summary?.TotalStudents ?? 0));
    private double StActiveArc  => (_summary?.ActiveStudents ?? 0) / StDenom * DonutCirc;
    private double StInactiveArc => ((_summary?.TotalStudents ?? 0) - (_summary?.ActiveStudents ?? 0)) / StDenom * DonutCirc;

    private string StDashActive   => StActiveArc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    private string StDashInactive => StInactiveArc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    private string StOffsetInactive => (DonutCirc - StActiveArc).ToString("F2", _inv);
    private string StCenterPct => ((_summary?.TotalStudents ?? 0) > 0
        ? (_summary!.ActiveStudents * 100.0 / _summary.TotalStudents)
        : 0.0).ToString("F0") + "%";

    // ── Courses donut ────────────────────────────────────────────────────────
    private double CoDenom      => Math.Max(1.0, (double)(_summary?.TotalCourses ?? 0));
    private double CoActiveArc  => (_summary?.ActiveCourses ?? 0) / CoDenom * DonutCirc;
    private double CoInactiveArc => ((_summary?.TotalCourses ?? 0) - (_summary?.ActiveCourses ?? 0)) / CoDenom * DonutCirc;

    private string CoDashActive   => CoActiveArc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    private string CoDashInactive => CoInactiveArc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    private string CoOffsetInactive => (DonutCirc - CoActiveArc).ToString("F2", _inv);
    private string CoCenterPct => ((_summary?.TotalCourses ?? 0) > 0
        ? (_summary!.ActiveCourses * 100.0 / _summary.TotalCourses)
        : 0.0).ToString("F0") + "%";

    // ────────────────────────────────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsInRole(LmsRoles.Admin))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadSummaryAsync();
    }

    private async Task LoadSummaryAsync()
    {
        try
        {
            _isLoading = true;
            _summary = await ReportingAppService.GetTenantSummaryAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoading = false;
        }
    }
}
