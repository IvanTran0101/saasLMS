using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace saasLMS.AssessmentService.Quizzes;

public interface IQuizAppService : IApplicationService
{
    Task<QuizDto> CreateAsync(CreateQuizDto dto);
    Task<QuizDto> UpdateAsync(Guid id, UpdateQuizDto input);
    Task PublishAsync(Guid id);
    Task CloseAsync(Guid id);
    Task<QuizDto> GetAsync(Guid id);
    Task<List<QuizListItemDto>> GetListByCourseAsync(Guid courseId);
    Task<List<QuizListItemDto>> GetListByLessonAsync(Guid lessonId);
    Task<List<QuizListItemDto>> GetListByLessonStudentAsync(Guid lessonId);
    Task<List<QuizListItemDto>> GetListByCourseStudentAsync(Guid lessonId);

}