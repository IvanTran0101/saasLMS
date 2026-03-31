using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Inputs;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using Volo.Abp.Application.Services;

namespace saasLMS.LearningProgressService.LessonProgresses;

public interface ILearningProgressAppService : IApplicationService
{
    Task<LessonProgressDto> StartLessonAsync(StartLessonInput input);
    Task<LessonProgressDto> ViewLessonAsync(ViewLessonInput input);
    Task<LessonProgressDto> CompleteLessonAsync(CompleteLessonInput input);
 
    Task<List<LessonProgressDto>> GetMyProgressAsync(Guid courseId);
    Task<CourseProgressDto> GetMyCourseProgressAsync(Guid courseId);
    Task<ResumeResultDto> GetResumePositionAsync(Guid courseId);
}