using System;
using Volo.Abp.Domain.Entities;

namespace saasLMS.ReportingService.ReadModels;

public class CourseOutcomeReportView : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }

    public int AssignmentGradedCount { get; set; }
    public decimal AssignmentScoreSum { get; set; }
    public decimal AvgAssignmentScore { get; set; }
    public int TotalAssignmentsCount { get; set; }

    public int QuizCompletedCount { get; set; }
    public decimal QuizScoreSum { get; set; }
    public decimal AvgQuizScore { get; set; }
    public int TotalQuizzesCount { get; set; }

    public int TotalLessonsCount { get; set; }

    public int FinalScoreCount { get; set; }
    public decimal FinalScoreSum { get; set; }
    public decimal FinalScoreAvg { get; set; }

    public decimal CompletionRate { get; set; }
    public decimal PassRate { get; set; }
    public string? ScoreDistributionJson { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    protected CourseOutcomeReportView()
    {
    }

    public CourseOutcomeReportView(Guid id, Guid tenantId, Guid courseId)
        : base(id)
    {
        TenantId = tenantId;
        CourseId = courseId;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
