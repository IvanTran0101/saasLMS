using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.QuizAttempts;

namespace saasLMS.AssessmentService.Controllers;

[Route("api/quiz-attempts")]
public class QuizAttemptController : AssessmentServiceController
{
    private readonly IQuizAttemptAppService _quizAttemptAppService;

    public QuizAttemptController(IQuizAttemptAppService quizAttemptAppService)
    {
        _quizAttemptAppService = quizAttemptAppService;
    }

    [HttpPost("start")]
    public Task<QuizAttemptDto> StartAsync(StartQuizAttemptDto input)
    {
        return _quizAttemptAppService.StartAsync(input);
    }

    [HttpPost("{quizId:guid}/submit")]
    public Task<QuizAttemptDto> SubmitAsync(Guid quizId, SubmitQuizAttemptDto input)
    {
        return _quizAttemptAppService.SubmitAsync(quizId, input);
    }

    [HttpPost("{quizId:guid}/timeout")]
    public Task<QuizAttemptDto> HandleTimeoutAsync(Guid quizId)
    {
        return _quizAttemptAppService.HandleTimeoutAsync(quizId);
    }

    [HttpGet("{quizId:guid}/mine")]
    public Task<QuizAttemptDto?> GetMyAttemptByQuizAsync(Guid quizId)
    {
        return _quizAttemptAppService.GetMyAttemptByQuizAsync(quizId);
    }

    [HttpGet("{id:guid}")]
    public Task<QuizAttemptDto> GetAsync(Guid id)
    {
        return _quizAttemptAppService.GetAsync(id);
    }

    [HttpGet("by-quiz/{quizId:guid}")]
    public Task<List<QuizAttemptDto>> GetListByQuizAsync(Guid quizId)
    {
        return _quizAttemptAppService.GetListByQuizAsync(quizId);
    }
}
