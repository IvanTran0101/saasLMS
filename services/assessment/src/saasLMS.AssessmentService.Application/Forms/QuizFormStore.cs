using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Forms;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Quizzes.Models;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Forms;
using Volo.Forms.Choices;
using Volo.Forms.Forms;
using Volo.Forms.Questions;

namespace saasLMS.AssessmentService.Forms;

public class QuizFormStore : IQuizFormStore, ITransientDependency
{
    private readonly IFormAppService _formAppService;
    private readonly IQuestionAppService _questionAppService;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public QuizFormStore(
        IFormAppService formAppService,
        IQuestionAppService questionAppService,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _formAppService = formAppService;
        _questionAppService = questionAppService;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<QuizFormSyncResult> CreateFormAsync(
        Quiz quiz,
        IReadOnlyList<QuizQuestion> questions,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        var form = await _formAppService.CreateAsync(new CreateFormDto
        {
            Title = quiz.Title,
            Description = string.Empty
        });
        if (_unitOfWorkManager.Current != null)
        {
            await _unitOfWorkManager.Current.SaveChangesAsync(cancellationToken);
        }

        await _formAppService.SetSettingsAsync(form.Id, new UpdateFormSettingInputDto
        {
            CanEditResponse = false,
            IsCollectingEmail = false,
            HasLimitOneResponsePerUser = false,
            IsAcceptingResponses = true,
            IsQuiz = true,
            RequiresLogin = true
        });

        var map = new Dictionary<int, Guid>();
        for (var i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            var created = await _formAppService.CreateQuestionAsync(form.Id, new CreateQuestionDto
            {
                Index = i,
                Title = question.Text,
                Description = string.Empty,
                IsRequired = true,
                HasOtherOption = false,
                QuestionType = QuestionTypes.ChoiceMultiple,
                Choices = BuildChoices(question.Answers, null)
            });
            map[i] = created.Id;
        }

        return new QuizFormSyncResult(form.Id, map);
    }

    public async Task<QuizFormSyncResult> UpdateFormAsync(
        Quiz quiz,
        IReadOnlyList<QuizQuestion> questions,
        DateTime now,
        CancellationToken cancellationToken = default)
    {
        if (!quiz.FormId.HasValue)
        {
            return await CreateFormAsync(quiz, questions, now, cancellationToken);
        }

        var form = await _formAppService.GetAsync(quiz.FormId.Value);
        await _formAppService.UpdateAsync(quiz.FormId.Value, new UpdateFormDto
        {
            Title = quiz.Title,
            Description = form.Description ?? string.Empty
        });

        await _formAppService.SetSettingsAsync(quiz.FormId.Value, new UpdateFormSettingInputDto
        {
            CanEditResponse = false,
            IsCollectingEmail = false,
            HasLimitOneResponsePerUser = false,
            IsAcceptingResponses = true,
            IsQuiz = true,
            RequiresLogin = true
        });

        var existingQuestions = await _formAppService.GetQuestionsAsync(
            quiz.FormId.Value,
            new GetQuestionListDto());

        var existingByIndex = existingQuestions.ToDictionary(x => x.Index);
        var usedIndexes = new HashSet<int>();
        var map = new Dictionary<int, Guid>();

        for (var i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            usedIndexes.Add(i);

            if (existingByIndex.TryGetValue(i, out var existing))
            {
                var updated = await _questionAppService.UpdateAsync(existing.Id, new UpdateQuestionDto
                {
                    Index = i,
                    Title = question.Text,
                    Description = existing.Description ?? string.Empty,
                    IsRequired = true,
                    HasOtherOption = false,
                    QuestionType = QuestionTypes.ChoiceMultiple,
                    Choices = BuildChoices(question.Answers, existing.Choices)
                });
                map[i] = updated.Id;
            }
            else
            {
                var created = await _formAppService.CreateQuestionAsync(quiz.FormId.Value, new CreateQuestionDto
                {
                    Index = i,
                    Title = question.Text,
                    Description = string.Empty,
                    IsRequired = true,
                    HasOtherOption = false,
                    QuestionType = QuestionTypes.ChoiceMultiple,
                    Choices = BuildChoices(question.Answers, null)
                });
                map[i] = created.Id;
            }
        }

        foreach (var existing in existingQuestions)
        {
            if (!usedIndexes.Contains(existing.Index))
            {
                await _questionAppService.DeleteAsync(existing.Id);
            }
        }

        return new QuizFormSyncResult(quiz.FormId.Value, map);
    }

    private static List<ChoiceDto> BuildChoices(
        IReadOnlyCollection<QuizAnswer> answers,
        IReadOnlyCollection<ChoiceDto>? existingChoices)
    {
        var existingByIndex = existingChoices?.ToDictionary(x => x.Index) ??
                              new Dictionary<int, ChoiceDto>();

        var result = new List<ChoiceDto>(answers.Count);
        var index = 0;
        foreach (var answer in answers)
        {
            var existingId = existingByIndex.TryGetValue(index, out var existing)
                ? existing.Id
                : Guid.Empty;

            result.Add(new ChoiceDto
            {
                Id = existingId,
                Index = index,
                Value = answer.Text,
                IsCorrect = answer.IsCorrect
            });
            index++;
        }

        return result;
    }
}
