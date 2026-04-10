using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Courses;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using saasLMS.AssessmentService.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Volo.Forms.Forms;
using Volo.Forms.Responses;

namespace saasLMS.AssessmentService;

public class QuizAttemptAppService : AssessmentServiceAppService, IQuizAttemptAppService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly QuizAttemptManager _quizAttemptManager;
    private readonly ICourseAccessChecker _courseAccessChecker;
    private readonly QuizGradingService _quizGradingService;
    private readonly IFormAppService _formAppService;
    private readonly IResponseAppService _responseAppService;

    public QuizAttemptAppService(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository,
        QuizAttemptManager quizAttemptManager,
        ICourseAccessChecker courseAccessChecker,
        QuizGradingService quizGradingService,
        IFormAppService formAppService,
        IResponseAppService responseAppService)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _quizAttemptManager = quizAttemptManager;
        _courseAccessChecker = courseAccessChecker;
        _quizGradingService = quizGradingService;
        _formAppService = formAppService;
        _responseAppService = responseAppService;
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.Start)]
    public async Task<QuizAttemptDto> StartAsync(StartQuizAttemptDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound"); 
        }

        if (input.QuizId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }
        var quiz =  await _quizRepository.GetAsync(input.QuizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        var quizAttempt = await _quizAttemptManager.StartAsync(
            quiz,
            tenantId.Value,
            studentId.Value,
            Clock.Now);
            quizAttempt = await _quizAttemptRepository.InsertAsync(quizAttempt, autoSave:true);
            return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.Submit)]
    public async Task<QuizAttemptDto> SubmitAsync(Guid quizId, SubmitQuizAttemptDto input)
    {
        if (quizId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(quizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }
        var quizAttempt = await _quizAttemptRepository.FindByQuizAndStudentAsync(
            tenantId.Value,
            quizId,
            studentId.Value);
        if (quizAttempt == null)
        {
            throw new BusinessException("AssessmentService:QuizAttemptNotFound")
                .WithData("QuizId", quiz.Id)
                .WithData("StudentId", studentId.Value);
        }
        if (input.Answers != null && input.Answers.Count > 0)
        {
            quiz.EnsureFormAttached();
            var response = await _responseAppService.SaveAnswersAsync(quiz.FormId!.Value, new CreateResponseDto
            {
                Email = CurrentUser.Email ?? string.Empty,
                Answers = input.Answers.Select(a => new Volo.Forms.Answers.CreateAnswerDto
                {
                    QuestionId = a.QuestionId,
                    ChoiceId = a.ChoiceId,
                    Value = a.Value ?? string.Empty
                }).ToList()
            });

            quizAttempt.AttachFormResponse(response.Id);

            var submittedAnswersJson = await BuildSubmittedAnswersJsonAsync(
                quiz.FormId!.Value,
                input.Answers);

            var scoreResult = await _quizGradingService.GradeAsync(
                quiz,
                quizAttempt,
                response.Id);
            quizAttempt.Complete(
                submittedAnswersJson,
                scoreResult.Score,
                Clock.Now);
        }
        else if (input.FormResponseId.HasValue)
        {
            quizAttempt.AttachFormResponse(input.FormResponseId.Value);
            var scoreResult = await _quizGradingService.GradeAsync(
                quiz,
                quizAttempt,
                input.FormResponseId.Value);
            quizAttempt.Complete(
                input.SubmittedAnswersJson,
                scoreResult.Score,
                Clock.Now);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(input.SubmittedAnswersJson))
            {
                throw new BusinessException("AssessmentService:QuizAttemptAnswersMissing");
            }
            await _quizAttemptManager.SubmitAndGradeAsync(
                quiz,
                quizAttempt,
                input.SubmittedAnswersJson,
                studentId.Value,
                Clock.Now);
        }
        await _quizAttemptRepository.UpdateAsync(quizAttempt, autoSave:true);
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    private async Task<string> BuildSubmittedAnswersJsonAsync(
        Guid formId,
        IReadOnlyList<QuizAttemptAnswerDto> answers)
    {
        var formQuestions = await _formAppService.GetQuestionsAsync(
            formId,
            new Volo.Forms.Questions.GetQuestionListDto());

        var questionIndexById = formQuestions.ToDictionary(q => q.Id, q => q.Index);
        var choiceIndexById = formQuestions
            .SelectMany(q => q.Choices?.Select(c => new { q.Id, Choice = c }) ?? Enumerable.Empty<dynamic>())
            .ToDictionary(x => (Guid)x.Choice.Id, x => (int)x.Choice.Index);

        var submitted = new List<QuizAttempts.Models.QuizSubmittedAnswerJsonModel>();
        foreach (var answer in answers)
        {
            if (!questionIndexById.TryGetValue(answer.QuestionId, out var qIndex))
            {
                continue;
            }
            if (!answer.ChoiceId.HasValue || !choiceIndexById.TryGetValue(answer.ChoiceId.Value, out var cIndex))
            {
                continue;
            }
            submitted.Add(new QuizAttempts.Models.QuizSubmittedAnswerJsonModel
            {
                QuestionIndex = qIndex,
                SelectedAnswerIndex = cIndex
            });
        }

        return JsonSerializer.Serialize(submitted);
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.View)]
    public async Task<QuizAttemptDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizAttemptIdIsEmpty");        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizAttempt = await _quizAttemptRepository.GetAsync(id);
        if (quizAttempt.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizAttemptTenantMismatch")
                .WithData("QuizAttemptId", quizAttempt.Id)
                .WithData("TenantId", tenantId.Value);
        }
        var quiz = await _quizRepository.GetAsync(quizAttempt.QuizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.ViewOwn)]
    public async Task<QuizAttemptDto?> GetMyAttemptByQuizAsync(Guid quizId)
    {
        if (quizId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(quizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }
        var quizAttempt = await _quizAttemptRepository.FindByQuizAndStudentAsync(
            tenantId.Value,
            quizId,
            studentId.Value);
        if (quizAttempt == null)
        {
            return null;
        }
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.View)]
    public async Task<List<QuizAttemptDto>> GetListByQuizAsync(Guid quizId)
    {
        if (quizId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(quizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        var quizAttempts = await _quizAttemptRepository.GetListByQuizAsync(
            tenantId.Value,
            quizId);
        
        return ObjectMapper.Map<List<QuizAttempt>, List<QuizAttemptDto>>(quizAttempts);
            
    }

    [Authorize(AssessmentServicePermissions.QuizAttempts.Submit)]
    public async Task<QuizAttemptDto> HandleTimeoutAsync(Guid quizId)
    {
        if (quizId == Guid.Empty) 
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(quizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        var quizAttempt = await _quizAttemptRepository.FindByQuizAndStudentAsync(
            tenantId.Value,
            quizId,
            studentId.Value);
        if (quizAttempt == null)
        {
            throw new BusinessException("AssessmentService:QuizAttemptNotFound")
                .WithData("QuizId", quiz.Id)
                .WithData("StudentId", studentId.Value);
        }
        await _quizAttemptManager.HandleTimeoutAsync(quiz, quizAttempt, Clock.Now);
        quizAttempt = await _quizAttemptRepository.UpdateAsync(quizAttempt, autoSave:true);
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }
}
