using System;

namespace saasLMS.ReportingService.Reports.Dtos.Outputs;

public class CourseOutcomeReportViewDto
{
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }

    public int AssignmentGradedCount { get; set; }
    public decimal AssignmentScoreSum { get; set; }
    public decimal AvgAssignmentScore { get; set; }

    public int QuizCompletedCount { get; set; }
    public decimal QuizScoreSum { get; set; }
    public decimal AvgQuizScore { get; set; }

    public int FinalScoreCount { get; set; }
    public decimal FinalScoreSum { get; set; }
    public decimal FinalScoreAvg { get; set; }

    public decimal CompletionRate { get; set; }
    public decimal PassRate { get; set; }
    public string? ScoreDistributionJson { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}
