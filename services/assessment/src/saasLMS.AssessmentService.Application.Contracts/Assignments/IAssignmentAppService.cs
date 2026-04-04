using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace saasLMS.AssessmentService.Assignments;

public interface IAssignmentAppService : IApplicationService
{
    Task<AssignmentDto> CreateAsync(CreateAssignmentDto input);
    Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input);
    Task PublishAsync(Guid id);
    Task CloseAsync(Guid id);
    Task<AssignmentDto> GetAsync(Guid id);
    Task<AssignmentDto> GetStudentAsync(Guid id);
    Task<List<AssignmentListItemDto>> GetListByCourseAsync(Guid courseId);
    Task<List<AssignmentListItemDto>> GetListByLessonAsync(Guid lessonId);
    Task<List<AssignmentListItemDto>> GetListByCourseStudentAsync(Guid courseId);
     Task<List<AssignmentListItemDto>> GetListByLessonStudentAsync(Guid lessonId);
}
