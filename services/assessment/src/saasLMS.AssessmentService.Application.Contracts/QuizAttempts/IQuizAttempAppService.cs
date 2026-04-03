using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace saasLMS.AssessmentService.QuizAttempts;

public interface IQuizAttemptAppService
{
    Task<QuizAttemptDto> StartAsync(StartQuizAttemptDto input);
    Task<QuizAttemptDto> SubmitAsync(Guid quizId, SubmitQuizAttemptDto input);
    Task<QuizAttemptDto> GetAsync(Guid id);
    Task<QuizAttemptDto?> GetMyAttemptByQuizAsync(Guid quizId);
    Task<List<QuizAttemptDto?>> GetListByQuizAsync(Guid quizId);
    Task<QuizAttemptDto> HandleTimeoutAsync(Guid quizId);
}