using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class QuizViewer : AbpComponentBase
{
    // ── Parameters ────────────────────────────────────────────────────────────

    [Parameter, EditorRequired] public QuizListItemDto Quiz { get; set; } = default!;
    [Parameter] public EventCallback OnDone { get; set; }

    // ── Services ──────────────────────────────────────────────────────────────

    [Inject] private IQuizAppService         QuizAppService         { get; set; } = default!;
    [Inject] private IQuizAttemptAppService  QuizAttemptAppService  { get; set; } = default!;

    // ── State ─────────────────────────────────────────────────────────────────

    private List<QuizAttemptDto>        _attempts        = new();
    private QuizAttemptDto?             _currentAttempt;
    private QuizFormSchemaDto?          _formSchema;

    /// Maps questionId → selected choiceId (for SingleChoice questions).
    private Dictionary<Guid, Guid>      _selectedChoices = new();

    /// Maps questionId → free-text value (for Text questions).
    private Dictionary<Guid, string>    _textAnswers     = new();

    private bool _isTakingQuiz = false;
    private bool _isLoading    = true;
    private bool _isStarting   = false;
    private bool _isSubmitting = false;

    /// <summary>True when the quiz is Closed — evaluated per render so the banner is always current.</summary>
    private bool _isLocked => Quiz.Status == QuizStatus.Closed;

    /// <summary>
    /// True when the student can (still) start/retry the quiz.
    /// Multiple-attempt quizzes allow re-taking; OneTime quizzes block once an attempt exists.
    /// </summary>
    private bool _canStart =>
        Quiz.Status == QuizStatus.Published &&
        (Quiz.AttemptPolicy == AttemptPolicy.Multiple || _attempts.Count == 0);

    /// <summary>
    /// Set when Start is clicked on a stale page (quiz was closed after load).
    /// Cleared on the next successful start.
    /// </summary>
    private string? _startWarning;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override async Task OnParametersSetAsync()
    {
        _isTakingQuiz    = false;
        _currentAttempt  = null;
        _formSchema      = null;
        _selectedChoices = new();
        _textAnswers     = new();
        _startWarning    = null;

        await LoadAttemptsAsync();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task LoadAttemptsAsync()
    {
        _isLoading = true;
        try
        {
            var myAttempt = await QuizAttemptAppService.GetMyAttemptByQuizAsync(Quiz.Id);
            _attempts = myAttempt is not null ? new() { myAttempt } : new();
        }
        catch
        {
            _attempts = new();
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task StartQuizAsync()
    {
        if (_isStarting) return;

        // Runtime re-check: quiz may have been closed since the page was loaded.
        if (Quiz.Status == QuizStatus.Closed)
        {
            _startWarning = "This quiz has been closed and is no longer accepting attempts. Please reload the page.";
            return;
        }

        _startWarning = null;
        _isStarting   = true;
        try
        {
            _currentAttempt  = await QuizAttemptAppService.StartAsync(new StartQuizAttemptDto { QuizId = Quiz.Id });
            _formSchema      = await QuizAppService.GetFormSchemaAsync(Quiz.Id);
            _selectedChoices = new();
            _textAnswers     = new();
            _isTakingQuiz    = true;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isStarting = false;
        }
    }

    private async Task SubmitQuizAsync()
    {
        if (_currentAttempt is null || _formSchema is null || _isSubmitting) return;
        _isSubmitting = true;
        try
        {
            var answers = _formSchema.Questions.Select(q => new QuizAttemptAnswerDto
            {
                QuestionId = q.Id,
                ChoiceId   = _selectedChoices.TryGetValue(q.Id, out var c) ? c : null,
                Value      = _textAnswers.TryGetValue(q.Id, out var v) ? v : null
            }).ToList();

            var submitDto = new SubmitQuizAttemptDto
            {
                Answers              = answers,
                SubmittedAnswersJson = JsonSerializer.Serialize(answers)
            };

            _currentAttempt = await QuizAttemptAppService.SubmitAsync(Quiz.Id, submitDto);
            _isTakingQuiz   = false;

            // Reload the updated attempt list so the score is reflected immediately
            await LoadAttemptsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private void SelectChoice(Guid questionId, Guid choiceId)
    {
        _selectedChoices[questionId] = choiceId;
    }

    private void SetTextAnswer(Guid questionId, string value)
    {
        _textAnswers[questionId] = value;
    }

    private static string FormatDuration(int minutes)
    {
        if (minutes < 60) return $"{minutes} min";
        var h = minutes / 60;
        var m = minutes % 60;
        return m == 0 ? $"{h} hr" : $"{h} hr {m} min";
    }

    /// <summary>Guarantees a DateTime is treated as UTC before any local-time conversion.</summary>
    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    /// <summary>Converts a UTC server timestamp to the browser's local time (e.g. GMT+7).</summary>
    private static string ToLocalDisplay(DateTime dt, string format = "MMM dd, yyyy HH:mm")
        => EnsureUtc(dt).ToLocalTime().ToString(format);
}
