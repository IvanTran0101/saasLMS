using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.QuizAttempts;
using Volo.Abp;

namespace saasLMS.AssessmentService;

[RemoteService(Name = AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Area(AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/assessment/quiz-attempts")]
public class QuizAttemptSubmitController : AssessmentServiceController
{
    private readonly IQuizAttemptAppService _quizAttemptAppService;

    public QuizAttemptSubmitController(IQuizAttemptAppService quizAttemptAppService)
    {
        _quizAttemptAppService = quizAttemptAppService;
    }

    /// <summary>
    /// Submit quiz attempt using Forms payload (preferred).
    /// </summary>
    /// <remarks>
    /// Example:
    /// {
    ///   "answers": [
    ///     {
    ///       "questionId": "00000000-0000-0000-0000-000000000000",
    ///       "choiceId": "00000000-0000-0000-0000-000000000000",
    ///       "value": null
    ///     }
    ///   ]
    /// }
    /// </remarks>
    [HttpPost("{quizId}/submit")]
    public Task<QuizAttemptDto> SubmitAsync(Guid quizId, [FromBody] SubmitQuizAttemptDto input)
    {
        return _quizAttemptAppService.SubmitAsync(quizId, input);
    }
}
