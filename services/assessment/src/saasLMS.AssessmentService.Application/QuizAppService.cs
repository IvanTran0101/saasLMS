using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;

namespace saasLMS.AssessmentService;

public class QuizAppService : AssessmentServiceAppService, IQuizAppService
{
    private readonly IQuizRepository _quizRepository;
    private readonly QuizManager _quizManager;

    public QuizAppService(IQuizRepository quizRepository, QuizManager quizManager)
    {
        _quizRepository = quizRepository;
        _quizManager = quizManager;
    }

    public async Task<QuizDto> CreateAsync(CreateQuizDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizManager.CreateAsync(
            tenantId.Value,
            input.CourseId,
            input.LessonId,
            input.Title,
            input.TimeLimitMinutes,
            input.MaxScore,
            input.AttemptPolicy,
            input.QuestionsJson);
        quiz = await _quizRepository.InsertAsync(quiz, autoSave: true);
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    public async Task<QuizDto> UpdateAsync(Guid id, UpdateQuizDto input)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        Check.NotNull(input, nameof(input));
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _quizManager.UpdateInfoAsync(
            quiz,
            input.Title,
            input.TimeLimitMinutes,
            input.MaxScore,
            input.AttemptPolicy,
            input.QuestionsJson);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    public async Task PublishAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _quizManager.PublishAsync(
            quiz,
            Clock.Now);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
    }

    public async Task CloseAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _quizManager.CloseAsync(
            quiz,
            Clock.Now);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
    }

    public async Task<QuizDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
        
    }

    public async Task<List<QuizListItemDto>> GetListByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByCourseAsync(tenantId.Value, courseId);
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }

    public async Task<List<QuizListItemDto>> GetListByLessonAsync(Guid lessonId)
    {
        
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByLessonAsync(tenantId.Value, lessonId);
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
        
    }

    public async Task<List<QuizListItemDto>> GetListByCourseStudentAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByCourseAsync(tenantId.Value, courseId);
        quizzes = quizzes.Where(
            x=> x.Status == QuizStatus.Published || x.Status == QuizStatus.Closed).ToList();
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }
    public async Task<List<QuizListItemDto>> GetListByLessonStudentAsync(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByLessonAsync(tenantId.Value, lessonId);
        quizzes = quizzes.Where(
            x=> x.Status == QuizStatus.Published || x.Status == QuizStatus.Closed).ToList();
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }
}