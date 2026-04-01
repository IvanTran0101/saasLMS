using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.QuizAttempts.Models;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Quizzes.Models;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.QuizAttempts;

public class QuizAttemptManager : DomainService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public QuizAttemptManager(IQuizAttemptRepository quizAttemptRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<QuizAttempt> StartAsync(
        Quiz quiz,
        Guid tenantId,
        Guid studentId,
        DateTime startedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        if (quiz.Status != QuizStatus.Published)
        {
            throw new BusinessException("AssessmentService:QuizNotAvailable")
                .WithData("QuizId", quiz.Id)
                .WithData("Status", quiz.Status);
        }
        
        var exists = await _quizAttemptRepository.ExistsByQuizAndStudentAsync(
            tenantId,
            quiz.Id,
            studentId,
            cancellationToken);
        
        if (quiz.AttemptPolicy == AttemptPolicy.OneTime && exists)
        {
            throw new BusinessException("AssessmentService:QuizAttemptLimitExceeded")
                .WithData("QuizId", quiz.Id)
                .WithData("StudentId", studentId);
        }

        var quizAttempt = QuizAttempt.Create(
            GuidGenerator.Create(),
            tenantId,
            quiz.Id,
            studentId,
            1,
            startedAt
        );
        return quizAttempt;
    }

    public Task SubmitAndGradeAsync(
        Quiz quiz,
        QuizAttempt quizAttempt,
        string submittedAnswersJson,
        Guid studentId,
        DateTime completedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        Check.NotNull(quizAttempt, nameof(quizAttempt));
        
        if (quiz.TenantId != quizAttempt.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizAttemptTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("QuizAttemptId", quizAttempt.Id);
        }
        
        if (quizAttempt.StudentId != studentId)
        {
            throw new BusinessException("AssessmentService:QuizAttemptStudentMismatch")
                .WithData("QuizAttemptId", quizAttempt.Id)
                .WithData("StudentId", studentId);
        }
        
        if (quizAttempt.QuizId != quiz.Id)
        {
            throw new BusinessException("AssessmentService:QuizAttemptDoesNotBelongToQuiz")
                .WithData("QuizId", quiz.Id)
                .WithData("QuizAttemptId", quizAttempt.Id);
        }
        
        if (quiz.Status != QuizStatus.Published)
        {
            throw new BusinessException("AssessmentService:QuizNotAvailable")
                .WithData("QuizId", quiz.Id)
                .WithData("Status", quiz.Status);
        }
        var questions = QuizQuestionsJsonValidator.ValidateAndParse(quiz.QuestionsJson);
        var submittedAnswers = QuizSubmittedAnswerJsonValidator.ValidateAndParse(submittedAnswersJson);

        ValidateSubmittedAnswersAgainstQuiz(questions, submittedAnswers);
        var score = CalculateScore(questions, submittedAnswers, quiz.MaxScore);

        if (quiz.TimeLimitMinutes.HasValue && completedAt > quizAttempt.StartedAt.AddMinutes(
                quiz.TimeLimitMinutes.Value))
        {
            throw new BusinessException("AssessmentService:QuizAttemptTimeExpired")
                .WithData("QuizId", quiz.Id)
                .WithData("QuizAttemptId", quizAttempt.Id);
        }        
        quizAttempt.Complete(submittedAnswersJson, score, completedAt);
        return Task.CompletedTask;
    }

    private static decimal CalculateScore(
        IReadOnlyList<QuizQuestion> questions,
        IReadOnlyList<QuizSubmittedAnswerJsonModel> submittedAnswers,
        decimal maxScore)
    {
        var correctCount = 0;
        foreach (var submittedAnswer in submittedAnswers)
        {
            var question = questions[submittedAnswer.QuestionIndex];
            var selectedAnswer = question.Answers.ElementAt(submittedAnswer.SelectedAnswerIndex);
            if (selectedAnswer.IsCorrect)
            {
                correctCount++;
            }
        }
        var totalQuestionCount = questions.Count;
        if (totalQuestionCount == 0)
        {
            return 0;
        }
        return Math.Round((decimal)correctCount / totalQuestionCount * maxScore, 2, MidpointRounding.AwayFromZero);    }
    
    private static void ValidateSubmittedAnswersAgainstQuiz(
        IReadOnlyList<QuizQuestion> questions,
        IReadOnlyList<QuizSubmittedAnswerJsonModel> submittedAnswers)
    {
        if (submittedAnswers.Count != questions.Count)
        {
            throw new BusinessException("AssessmentService:IncompleteQuizSubmission")
                .WithData("ExpectedQuestionCount", questions.Count)
                .WithData("ActualSubmittedCount", submittedAnswers.Count);
        }
        foreach (var submittedAnswer in submittedAnswers)
        {
            if (submittedAnswer.QuestionIndex >= questions.Count)
            {
                throw new BusinessException("AssessmentService:InvalidQuestionIndex")
                    .WithData("QuestionIndex", submittedAnswer.QuestionIndex);
            }
            var question = questions[submittedAnswer.QuestionIndex];
            var answers = question.Answers.ToList();

            if (submittedAnswer.SelectedAnswerIndex >= answers.Count)
            {
                throw new BusinessException("AssessmentService:InvalidSelectedAnswerIndex")
                    .WithData("QuestionIndex", submittedAnswer.QuestionIndex)
                    .WithData("SelectedAnswerIndex", submittedAnswer.SelectedAnswerIndex);
            }
        }
    }
    
    

    public Task ExpireAsync(QuizAttempt quizAttempt,
        DateTime expiredAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quizAttempt, nameof(quizAttempt));
        quizAttempt.Expire(expiredAt);
        return Task.CompletedTask;
    }
}