using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class QuizViewer : AbpComponentBase, IAsyncDisposable
{
    // ── Parameters ────────────────────────────────────────────────────────────

    [Parameter, EditorRequired] public QuizListItemDto Quiz { get; set; } = default!;
    [Parameter] public EventCallback OnDone { get; set; }

    /// <summary>
    /// Fired with <c>true</c> when the student starts taking the quiz,
    /// and <c>false</c> when they finish/leave. The parent uses this to
    /// guard its own navigation controls.
    /// </summary>
    [Parameter] public EventCallback<bool> OnTakingQuizChanged { get; set; }

    // ── Services ──────────────────────────────────────────────────────────────

    [Inject] private IQuizAppService        QuizAppService        { get; set; } = default!;
    [Inject] private IQuizAttemptAppService QuizAttemptAppService { get; set; } = default!;
    [Inject] private NavigationManager      NavigationManager     { get; set; } = default!;
    [Inject] private IJSRuntime             JS                    { get; set; } = default!;

    // ── Quiz state ────────────────────────────────────────────────────────────

    private List<QuizAttemptDto>     _attempts        = new();
    private QuizAttemptDto?          _currentAttempt;
    private QuizFormSchemaDto?       _formSchema;
    private Dictionary<Guid, Guid>   _selectedChoices = new();
    private Dictionary<Guid, string> _textAnswers     = new();

    private bool _isTakingQuiz = false;
    private bool _isLoading    = true;
    private bool _isStarting   = false;
    private bool _isSubmitting = false;

    private bool _isLocked => Quiz.Status == QuizStatus.Closed;
    private bool _canStart =>
        Quiz.Status == QuizStatus.Published &&
        (Quiz.AttemptPolicy == AttemptPolicy.Multiple || _attempts.Count == 0);
 
    private string? _startWarning;

    // ── Timer state ───────────────────────────────────────────────────────────

    private int                    _remainingSeconds;
    private bool                   _isTimedOut = false;
    private CancellationTokenSource? _countdownCts;

    private bool HasTimer => Quiz.TimeLimitMinutes is > 0;

    private double TimerProgressPct => HasTimer
        ? Math.Max(0d, (double)_remainingSeconds / (Quiz.TimeLimitMinutes!.Value * 60) * 100d)
        : 100d;

    private string TimerCssClass => _remainingSeconds switch
    {
        <= 60  => "qv-timer--critical",
        <= 300 => "qv-timer--warning",
        _      => ""
    };

    private string FormatRemaining()
    {
        var m = _remainingSeconds / 60;
        var s = _remainingSeconds % 60;
        return $"{m:D2}:{s:D2}";
    }

    // ── Leave-guard state ─────────────────────────────────────────────────────

    private bool         _showLeaveConfirm = false;
    private IDisposable? _locationChangingReg;

    // ── Identity tracking (prevents re-init on every parent render) ───────────

    private Guid _loadedQuizId = Guid.Empty;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override async Task OnParametersSetAsync()
    {
        // Blazor calls OnParametersSetAsync on every parent render, not only when
        // parameters actually changed.  Guard against redundant resets that would
        // cause an infinite render loop (InvokeAsync → parent re-renders → repeat).
        if (Quiz.Id == _loadedQuizId) return;
        _loadedQuizId = Quiz.Id;

        // Quiz actually changed — tear down any active session cleanly.
        await StopCountdownAsync();
        _locationChangingReg?.Dispose();
        _locationChangingReg = null;

        _isTakingQuiz     = false;
        _currentAttempt   = null;
        _formSchema       = null;
        _selectedChoices  = new();
        _textAnswers      = new();
        _startWarning     = null;
        _showLeaveConfirm = false;
        _isTimedOut       = false;

        // Do NOT invoke OnTakingQuizChanged here — the parent already knows the
        // value is false (it chose the new quiz), and calling InvokeAsync would
        // trigger a parent re-render → OnParametersSetAsync again → loop.

        await LoadAttemptsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await StopCountdownAsync();
        _locationChangingReg?.Dispose();
    }

    // ── Data ──────────────────────────────────────────────────────────────────

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

    // ── Quiz actions ──────────────────────────────────────────────────────────

    private async Task StartQuizAsync()
    {
        if (_isStarting) return;

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
            _isTimedOut      = false;

            _isTakingQuiz = true;
            if (OnTakingQuizChanged.HasDelegate)
                await OnTakingQuizChanged.InvokeAsync(true);

            RegisterLocationGuard();

            if (HasTimer)
            {
                _remainingSeconds = Quiz.TimeLimitMinutes!.Value * 60;
                _ = RunCountdownAsync();
            }
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
            await StopCountdownAsync();   // stop timer before hitting server

            var answers = _formSchema.Questions.Select(q => new QuizAttemptAnswerDto
            {
                QuestionId = q.Id,
                ChoiceId   = _selectedChoices.TryGetValue(q.Id, out var c) ? c : null,
                Value      = _textAnswers.TryGetValue(q.Id, out var v) ? v : null
            }).ToList();

            _currentAttempt = await QuizAttemptAppService.SubmitAsync(Quiz.Id, new SubmitQuizAttemptDto
            {
                Answers              = answers,
                SubmittedAnswersJson = JsonSerializer.Serialize(answers)
            });

            await DeactivateTakingQuizAsync();
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

    // ── Timer ─────────────────────────────────────────────────────────────────

    private async Task RunCountdownAsync()
    {
        _countdownCts = new CancellationTokenSource();
        var token = _countdownCts.Token;

        try
        {
            while (_remainingSeconds > 0 && !token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);
                _remainingSeconds--;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }

        // Expired naturally (not cancelled) — handle server-side timeout
        if (!token.IsCancellationRequested)
            await InvokeAsync(HandleTimerExpiredAsync);
    }

    private async Task StopCountdownAsync()
    {
        if (_countdownCts is not null)
        {
            await _countdownCts.CancelAsync();
            _countdownCts.Dispose();
            _countdownCts = null;
        }
    }

    private async Task HandleTimerExpiredAsync()
    {
        if (!_isTakingQuiz) return;   // already submitted/cancelled

        _isTimedOut = true;
        StateHasChanged();

        // Submit current answers so nothing is lost; SubmitQuizAsync handles
        // DeactivateTakingQuizAsync + LoadAttemptsAsync internally.
        await SubmitQuizAsync();
    }

    // ── Leave-guard helpers ───────────────────────────────────────────────────

    private void RegisterLocationGuard()
    {
        _locationChangingReg?.Dispose();
        _locationChangingReg = NavigationManager.RegisterLocationChangingHandler(async context =>
        {
            if (!_isTakingQuiz) return;

            var confirmed = await JS.InvokeAsync<bool>(
                "confirm",
                "You are currently taking a quiz. If you leave this page, your answers will not be submitted.\n\nAre you sure you want to leave?");

            if (!confirmed)
                context.PreventNavigation();
        });
    }

    private async Task DeactivateTakingQuizAsync()
    {
        _isTakingQuiz     = false;
        _showLeaveConfirm = false;
        _locationChangingReg?.Dispose();
        _locationChangingReg = null;

        if (OnTakingQuizChanged.HasDelegate)
            await OnTakingQuizChanged.InvokeAsync(false);
    }

    /// <summary>Called by the Back button while taking a quiz — shows inline confirmation.</summary>
    private void RequestLeave()
    {
        if (_isTakingQuiz)
            _showLeaveConfirm = true;
        else if (OnDone.HasDelegate)
            _ = OnDone.InvokeAsync();
    }

    private async Task ConfirmLeaveAsync()
    {
        _showLeaveConfirm = false;
        await StopCountdownAsync();
        await DeactivateTakingQuizAsync();  // sets _quizInProgress=false in parent BEFORE OnDone fires
        if (OnDone.HasDelegate)
            await OnDone.InvokeAsync();
    }

    private void CancelLeave() => _showLeaveConfirm = false;

    // ── Answer helpers ────────────────────────────────────────────────────────

    private void SelectChoice(Guid questionId, Guid choiceId)
        => _selectedChoices[questionId] = choiceId;

    private void SetTextAnswer(Guid questionId, string value)
        => _textAnswers[questionId] = value;

    // ── Formatting ────────────────────────────────────────────────────────────

    private static string FormatDuration(int minutes)
    {
        if (minutes < 60) return $"{minutes} min";
        var h = minutes / 60;
        var m = minutes % 60;
        return m == 0 ? $"{h} hr" : $"{h} hr {m} min";
    }

    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    private static string ToLocalDisplay(DateTime dt, string format = "MMM dd, yyyy HH:mm")
        => EnsureUtc(dt).ToLocalTime().ToString(format);
}
