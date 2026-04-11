using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Forms;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Forms.Answers;
using Volo.Forms.Questions;
using Volo.Forms.Responses;

namespace saasLMS.AssessmentService.Forms;

public class QuizFormResponseReader : IQuizFormResponseReader, ITransientDependency
{
    private readonly IResponseAppService _responseAppService;
    private readonly IQuestionAppService _questionAppService;

    public QuizFormResponseReader(
        IResponseAppService responseAppService,
        IQuestionAppService questionAppService)
    {
        _responseAppService = responseAppService;
        _questionAppService = questionAppService;
    }

    public async Task<IReadOnlyList<QuizFormAnswer>> GetAnswersAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default)
    {
        var questionsWithAnswers = (IReadOnlyList<dynamic>?)null;
        try
        {
            // Use type inference to avoid referencing DTOs that may not be in this project.
            questionsWithAnswers = await _responseAppService.GetQuestionsWithAnswersAsync(formResponseId);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements"))
        {
            throw new BusinessException("AssessmentService:FormResponseHasNoAnswers")
                .WithData("FormResponseId", formResponseId);
        }
        if (questionsWithAnswers == null)
        {
            throw new BusinessException("AssessmentService:FormResponseNotFound")
                .WithData("FormResponseId", formResponseId);
        }
        var result = new List<QuizFormAnswer>(questionsWithAnswers.Count);

        foreach (var question in questionsWithAnswers)
        {
            var answers = question.Answers as IEnumerable<AnswerDto>;
            var answer = answers?.FirstOrDefault();
            result.Add(new QuizFormAnswer(
                question.Id,
                answer?.ChoiceId,
                answer?.Value));
        }

        return result;
    }

    public async Task<IReadOnlyList<QuizFormQuestionKey>> GetQuestionKeysAsync(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken cancellationToken = default)
    {
        var result = new List<QuizFormQuestionKey>(questionIds.Count);

        foreach (var questionId in questionIds)
        {
            var question = await _questionAppService.GetAsync(questionId);
            var correctChoice = question.Choices?.FirstOrDefault(x => x.IsCorrect);
            result.Add(new QuizFormQuestionKey(
                question.Id,
                correctChoice?.Id,
                null));
        }

        return result;
    }
}
