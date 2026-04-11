using System;
using Microsoft.AspNetCore.Http;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Quizzes;

public class UploadQuizCsvRequest
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public decimal MaxScore { get; set; }
    public AttemptPolicy AttemptPolicy { get; set; }
    public IFormFile? File { get; set; }
}
