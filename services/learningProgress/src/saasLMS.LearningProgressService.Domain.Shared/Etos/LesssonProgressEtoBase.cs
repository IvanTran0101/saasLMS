using System;

namespace saasLMS.LearningProgressService.Etos;

public abstract class LessonProgressEtoBase
{
    public Guid EventId { get; init; }

    public DateTime OccurredAt { get; init; }

    public Guid? TenantId { get; init; }

    public Guid StudentId { get; init; }

    public Guid CourseId { get; init; }

    public Guid LessonId { get; init; }
}