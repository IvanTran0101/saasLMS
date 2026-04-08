using System;

namespace saasLMS.ReportingService.Reports.Dtos.Outputs;

public class StudentCourseProgressViewDto
{
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }

    public int CompletedLessonsCount { get; set; }
    public int TotalLessonsCount { get; set; }
    public decimal LessonCompletionPercent { get; set; }

    public int AssignmentGradedCount { get; set; }
    public int TotalAssignmentsCount { get; set; }
    public decimal AssignmentScoreSum { get; set; }
    public decimal AssignmentCompletionPercent { get; set; }
    public decimal AvgAssignmentScore { get; set; }

    public int QuizCompletedCount { get; set; }
    public int TotalQuizzesCount { get; set; }
    public decimal QuizScoreSum { get; set; }
    public decimal QuizCompletionPercent { get; set; }
    public decimal AvgQuizScore { get; set; }

    public decimal OverallProgress { get; set; }
    public Guid? LastAccessedLessonId { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
