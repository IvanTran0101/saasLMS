using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using saasLMS.AssessmentService.QuizAttempts;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class QuizResultModal : AbpComponentBase
{
    // ── Dependencies ──────────────────────────────────────────────────────────────

    [Inject]
    private IQuizAttemptAppService QuizAttemptAppService { get; set; } = default!; 

    // ── State ─────────────────────────────────────────────────────────────────────

    private bool    _isVisible;
    private bool    _isLoading;

    private Guid    _quizId;
    private string  _quizTitle = string.Empty;
    private decimal _maxScore;

    private List<QuizAttemptDto> _attempts = new();

    // ── Public API ────────────────────────────────────────────────────────────────

    public void Show(Guid quizId, string quizTitle, decimal maxScore)
    {
        _quizId    = quizId;
        _quizTitle = quizTitle;
        _maxScore  = maxScore;
        _isVisible = true;
        _attempts  = new List<QuizAttemptDto>();

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
}