using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Quizzes;

namespace saasLMS.AssessmentService.Controllers;

[Route("api/quizzes")]
public class QuizController : AssessmentServiceController
{
    private readonly IQuizAppService _quizAppService;

    public QuizController(IQuizAppService quizAppService)
    {
        _quizAppService = quizAppService;
    }

    [HttpPost]
    public Task<QuizDto> CreateAsync(CreateQuizDto input)
    {
        return _quizAppService.CreateAsync(input);
    }

    [HttpPut("{id:guid}")]
    public Task<QuizDto> UpdateAsync(Guid id, UpdateQuizDto input)
    {
        return _quizAppService.UpdateAsync(id, input);
    }

    [HttpPost("{id:guid}/publish")]
    public Task PublishAsync(Guid id)
    {
        return _quizAppService.PublishAsync(id);
    }

    [HttpPost("{id:guid}/close")]
    public Task CloseAsync(Guid id)
    {
        return _quizAppService.CloseAsync(id);
    }

    [HttpGet("{id:guid}")]
    public Task<QuizDto> GetAsync(Guid id)
    {
        return _quizAppService.GetAsync(id);
    }

    [HttpGet("{id:guid}/student")]
    public Task<QuizDto> GetStudentAsync(Guid id)
    {
        return _quizAppService.GetStudentAsync(id);
    }

    [HttpGet("by-course/{courseId:guid}")]
    public Task<List<QuizListItemDto>> GetListByCourseAsync(Guid courseId)
    {
        return _quizAppService.GetListByCourseAsync(courseId);
    }

    [HttpGet("by-lesson/{lessonId:guid}")]
    public Task<List<QuizListItemDto>> GetListByLessonAsync(Guid lessonId)
    {
        return _quizAppService.GetListByLessonAsync(lessonId);
    }

    [HttpGet("by-course/{courseId:guid}/student")]
    public Task<List<QuizListItemDto>> GetListByCourseStudentAsync(Guid courseId)
    {
        return _quizAppService.GetListByCourseStudentAsync(courseId);
    }

    [HttpGet("by-lesson/{lessonId:guid}/student")]
    public Task<List<QuizListItemDto>> GetListByLessonStudentAsync(Guid lessonId)
    {
        return _quizAppService.GetListByLessonStudentAsync(lessonId);
    }
}
