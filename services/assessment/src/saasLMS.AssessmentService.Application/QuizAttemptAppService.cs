using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;

namespace saasLMS.AssessmentService;

public class QuizAttemptAppService : AssessmentServiceAppService, IQuizAttemptAppService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly QuizAttemptManager _quizAttemptManager;

    public QuizAttemptAppService( IQuizRepository quizRepository,IQuizAttemptRepository quizAttemptRepository, QuizAttemptManager quizAttemptManager)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _quizAttemptManager = quizAttemptManager;
    }

    public async Task<QuizAttemptDto> StartAsync(StartQuizAttemptDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound"); 
        }

        if (input.QuizId != Guid.Empty)
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

    public async Task<QuizAttemptDto> SubmitAsync(Guid quizId, SubmitQuizAttemptDto input)
    {
        if (quizId != Guid.Empty)
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
        var quizAttempt = await _quizAttemptRepository.GetAsync(quizId);
        await _quizAttemptManager.SubmitAndGradeAsync(
            quiz,
            quizAttempt,
            input.SubmittedAnswerJson,
            studentId.Value,
            Clock.Now);
        await _quizAttemptRepository.UpdateAsync(quizAttempt, autoSave:true);
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    public async Task<QuizAttemptDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizAttemptIdNotFound");
        }
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
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
    }

    public async Task<QuizAttemptDto?> GetMyAttemptByQuizAsync(Guid quizId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<QuizAttemptDto?>> GetListByQuizAsync(Guid quizId)
    {
        throw new NotImplementedException();
    }

    public async Task<QuizAttemptDto> HandleTimeoutAsync(Guid quizId)
    {
        throw new NotImplementedException();
    }
}