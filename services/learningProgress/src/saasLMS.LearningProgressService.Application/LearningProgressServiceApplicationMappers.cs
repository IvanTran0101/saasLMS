using Riok.Mapperly.Abstractions;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using Volo.Abp.Mapperly;

namespace saasLMS.LearningProgressService;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LessonProgressToLessonProgressDtoMapper : MapperBase<LessonProgress, LessonProgressDto>
{
    public override partial LessonProgressDto Map(LessonProgress source);
    public override partial void Map(LessonProgress source, LessonProgressDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LessonProgressToResumeResultDtoMapper : MapperBase<LessonProgress, ResumeResultDto>
{
    [MapProperty(nameof(LessonProgress.CourseId), nameof(ResumeResultDto.CourseId))]
    [MapProperty(nameof(LessonProgress.LessonId), nameof(ResumeResultDto.LessonId))]
    [MapProperty(nameof(LessonProgress.Status), nameof(ResumeResultDto.LessonStatus))]
    public override partial ResumeResultDto Map(LessonProgress source);
    [MapProperty(nameof(LessonProgress.CourseId), nameof(ResumeResultDto.CourseId))]
    [MapProperty(nameof(LessonProgress.LessonId), nameof(ResumeResultDto.LessonId))]
    [MapProperty(nameof(LessonProgress.Status), nameof(ResumeResultDto.LessonStatus))]
    public override partial void Map(LessonProgress source, ResumeResultDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CourseProgressToCourseProgressDtoMapper : MapperBase<CourseProgress, CourseProgressDto>
{
    public override partial CourseProgressDto Map(CourseProgress source);
    public override partial void Map(CourseProgress source, CourseProgressDto destination);
}
