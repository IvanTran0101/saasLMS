using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using saasLMS.AssessmentService.QuizAttempts;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Identity;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class QuizResultModal : AbpComponentBase
{
    // ── Dependencies ──────────────────────────────────────────────────────────────

    [Inject]
    private IQuizAttemptAppService QuizAttemptAppService { get; set; } = default!;

    [Inject]
    private IIdentityUserAppService IdentityUserAppService { get; set; } = default!;

    // ── State ─────────────────────────────────────────────────────────────────────

    private bool    _isVisible;
    private bool    _isLoading;

    private Guid    _quizId;
    private string  _quizTitle = string.Empty;
    private decimal _maxScore;

    private List<QuizAttemptDto>     _attempts        = new();
    private Dictionary<Guid, string> _studentNameMap  = new();

    // ── Public API ────────────────────────────────────────────────────────────────

    public void Show(Guid quizId, string quizTitle, decimal maxScore)
    {
        _quizId         = quizId;
        _quizTitle      = quizTitle;
        _maxScore       = maxScore;
        _isVisible      = true;
        _attempts       = new List<QuizAttemptDto>();
        _studentNameMap = new Dictionary<Guid, string>();

        StateHasChanged();
        _ = LoadAttemptsAsync();
    }

    public void Close()
    {
        _isVisible = false;
        StateHasChanged();
    }

    // ── Private Methods ───────────────────────────────────────────────────────────

    private async Task LoadAttemptsAsync()
    {
        try
        {
            _isLoading = true;
            StateHasChanged();

            var list = await QuizAttemptAppService.GetListByQuizAsync(_quizId);
            _attempts = list ?? new List<QuizAttemptDto>();

            await LoadStudentNamesAsync();
        }
        catch (Exception)
        {
            _attempts = new List<QuizAttemptDto>();
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadStudentNamesAsync()
    {
        var studentIds = _attempts.Select(a => a.StudentId).Distinct().ToList();
        if (studentIds.Count == 0) return;

        var tasks = studentIds.Select(async id =>
        {
            try
            {
                var user = await IdentityUserAppService.GetAsync(id);
                var fullName = $"{user.Name} {user.Surname}".Trim();
                return (id, name: string.IsNullOrEmpty(fullName) ? user.UserName : fullName);
            }
            catch
            {
                return (id, name: "Student");
            }
        });

        var results = await Task.WhenAll(tasks);
        _studentNameMap = results.ToDictionary(r => r.id, r => r.name);
    }

    private string GetStudentName(Guid studentId)
        => _studentNameMap.TryGetValue(studentId, out var name) ? name : "Student";
}