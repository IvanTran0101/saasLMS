using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class GradingModal : AbpComponentBase
{
    // ── Dependencies ──────────────────────────────────────────────────────────────

    [Inject]
    private ISubmissionAppService SubmissionAppService { get; set; } = default!;

    // ── State ─────────────────────────────────────────────────────────────────────

    private bool _isVisible;
    private bool _isLoading;

    private Guid    _assignmentId;
    private string  _assignmentTitle = string.Empty;
    private decimal _maxScore;

    private List<SubmissionListItemDto> _submissions = new();

    /// <summary>Raw text input per submission ID (before parsing to decimal).</summary>
    private readonly Dictionary<Guid, string> _scoreInputs = new();

    /// <summary>Which submission is currently being saved.</summary>
    private Guid? _gradingSubmissionId;

    // ── Public API ────────────────────────────────────────────────────────────────

    public void Show(Guid assignmentId, string assignmentTitle, decimal maxScore)
    {
        _assignmentId    = assignmentId;
        _assignmentTitle = assignmentTitle;
        _maxScore        = maxScore;
        _isVisible       = true;
        _submissions     = new List<SubmissionListItemDto>();
        _scoreInputs.Clear();
        _gradingSubmissionId = null;

        StateHasChanged();
        _ = LoadSubmissionsAsync();
    }

    public void Close()
    {
        _isVisible = false;
        StateHasChanged();
    }

    // ── Private Methods ───────────────────────────────────────────────────────────

    private async Task LoadSubmissionsAsync()
    {
        try
        {
            _isLoading = true;
            StateHasChanged();

            var list = await SubmissionAppService.GetListByAssignmentAsync(_assignmentId);
            _submissions = list ?? new List<SubmissionListItemDto>();

            // Pre-fill score inputs for already-graded submissions (disabled)
            foreach (var sub in _submissions.Where(s => s.Status == SubmissionStatus.Graded && s.Score.HasValue))
            {
                _scoreInputs[sub.Id] = sub.Score!.Value.ToString("G");
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            _submissions = new List<SubmissionListItemDto>();
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void OnScoreInput(Guid submissionId, string? value)
    {
        _scoreInputs[submissionId] = value ?? string.Empty;
    }

    private async Task GradeSubmissionAsync(Guid submissionId)
    {
        if (!_scoreInputs.TryGetValue(submissionId, out var raw) || string.IsNullOrWhiteSpace(raw))
            return;

        if (!decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var score))
        {
            await HandleErrorAsync(new Exception("Invalid score value. Please enter a valid number."));
            return;
        }

        try
        {
            _gradingSubmissionId = submissionId;
            StateHasChanged();

            var result = await SubmissionAppService.GradeAsync(submissionId, new GradeSubmissionDto { Score = score });

            // Update local DTO so the row flips to graded state
            var sub = _submissions.FirstOrDefault(s => s.Id == submissionId);
            if (sub is not null)
            {
                sub.Status   = SubmissionStatus.Graded;
                sub.Score    = result.Score;
                sub.GradedAt = result.GradedAt;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _gradingSubmissionId = null;
            StateHasChanged();
        }
    }
}
