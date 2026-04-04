using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Courses;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using saasLMS.AssessmentService.Permissions;

namespace saasLMS.AssessmentService;

public class QuizAttemptAppService : AssessmentServiceAppService, IQuizAttemptAppService
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly QuizAttemptManager _quizAttemptManager;
    private readonly ICourseAccessChecker _courseAccessChecker;

    public QuizAttemptAppService(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository,
        QuizAttemptManager quizAttemptManager,
        ICourseAccessChecker courseAccessChecker)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _quizAttemptManager = quizAttemptManager;
        _courseAccessChecker = courseAccessChecker;
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
        await _quizAttemptManager.SubmitAndGradeAsync(
            quiz,
            quizAttempt,
            input.SubmittedAnswersJson,
            studentId.Value,
            Clock.Now);
        await _quizAttemptRepository.UpdateAsync(quizAttempt, autoSave:true);
        return ObjectMapper.Map<QuizAttempt, QuizAttemptDto>(quizAttempt);
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
