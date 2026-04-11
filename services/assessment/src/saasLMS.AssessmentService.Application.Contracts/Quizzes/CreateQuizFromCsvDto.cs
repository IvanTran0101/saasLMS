using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Quizzes;

public class CreateQuizFromCsvDto
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public decimal MaxScore { get; set; }
    public AttemptPolicy AttemptPolicy { get; set; }
} 
