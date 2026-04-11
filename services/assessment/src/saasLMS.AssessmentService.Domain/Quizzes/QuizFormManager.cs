using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Forms;
using saasLMS.AssessmentService.Quizzes.Models;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.Quizzes;

public class QuizFormManager : DomainService
{
    private readonly IQuizFormStore _quizFormStore;
    private readonly IRepository<QuizQuestionMap, Guid> _quizQuestionMapRepository;

    public QuizFormManager(
        IQuizFormStore quizFormStore,
        IRepository<QuizQuestionMap, Guid> quizQuestionMapRepository)
    {
        _quizFormStore = quizFormStore;
        _quizQuestionMapRepository = quizQuestionMapRepository;
    }

    public async Task<QuizFormSyncResult> SyncFormAsync(
        Quiz quiz,
        string questionsJson,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        var questions = QuizQuestionsJsonValidator.ValidateAndParse(questionsJson);

        QuizFormSyncResult result;
        if (quiz.FormId.HasValue)
        {
            result = await _quizFormStore.UpdateFormAsync(
                quiz,
                questions,
                now,
                cancellationToken);
        }
        else
        {
            result = await _quizFormStore.CreateFormAsync(
                quiz,
                questions,
                now,
                cancellationToken);
            quiz.AttachForm(result.FormId);
        }

        await SyncQuestionMapsAsync(quiz, result, cancellationToken);
        return result;
    }

    public async Task<QuizFormSyncResult> CreateFormForNewQuizAsync(
        Quiz quiz,
        string questionsJson,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        if (quiz.FormId.HasValue)
        {
            throw new BusinessException("AssessmentService:QuizAlreadyHasForm")
                .WithData("QuizId", quiz.Id);
        }

        var questions = QuizQuestionsJsonValidator.ValidateAndParse(questionsJson);
        var result = await _quizFormStore.CreateFormAsync(
            quiz,
            questions,
            now,
            cancellationToken);
        quiz.AttachForm(result.FormId);
        return result;
    }

    public async Task SyncQuestionMapsAsync(
        Quiz quiz,
        QuizFormSyncResult result,
        CancellationToken cancellationToken)
    {
        var existing = await _quizQuestionMapRepository.GetListAsync(
            x => x.QuizId == quiz.Id && x.TenantId == quiz.TenantId,
            includeDetails: false,
            cancellationToken: cancellationToken);

        var existingByIndex = existing.ToDictionary(x => x.QuestionIndex);
        var keepIndexes = new HashSet<int>();

        foreach (var pair in result.QuestionIdByIndex)
        {
            keepIndexes.Add(pair.Key);
            if (existingByIndex.TryGetValue(pair.Key, out var map))
            {
                if (map.FormQuestionId != pair.Value)
                {
                    map.UpdateFormQuestion(pair.Value);
                }
                if (map.QuestionIndex != pair.Key)
                {
                    map.UpdateQuestionIndex(pair.Key);
                }
            }
            else
            {
                var newMap = new QuizQuestionMap(
                    GuidGenerator.Create(),
                    quiz.TenantId,
                    quiz.Id,
                    pair.Value,
                    pair.Key);
                await _quizQuestionMapRepository.InsertAsync(newMap, cancellationToken: cancellationToken);
            }
        }

        foreach (var map in existing)
        {
            if (!keepIndexes.Contains(map.QuestionIndex))
            {
                await _quizQuestionMapRepository.DeleteAsync(map, cancellationToken: cancellationToken);
            }
        }
    }
}
