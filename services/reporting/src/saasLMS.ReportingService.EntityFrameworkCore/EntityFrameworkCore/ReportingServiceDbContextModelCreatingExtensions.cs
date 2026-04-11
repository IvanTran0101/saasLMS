using Microsoft.EntityFrameworkCore;
using saasLMS.ReportingService.ReadModels;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace saasLMS.ReportingService.EntityFrameworkCore;

public static class ReportingServiceDbContextModelCreatingExtensions
{
    public static void ConfigureReportingService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<StudentCourseProgressView>(b =>
        {
            b.ToTable(ReportingServiceDbProperties.DbTablePrefix + "StudentCourseProgressViews", ReportingServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.IsActiveEnrollment).IsRequired();
            b.Property(x => x.LessonCompletionPercent).HasPrecision(5, 2);
            b.Property(x => x.AssignmentScoreSum).HasPrecision(18, 2);
            b.Property(x => x.AssignmentCompletionPercent).HasPrecision(5, 2);
            b.Property(x => x.AvgAssignmentScore).HasPrecision(18, 2);
            b.Property(x => x.QuizScoreSum).HasPrecision(18, 2);
            b.Property(x => x.QuizCompletionPercent).HasPrecision(5, 2);
            b.Property(x => x.AvgQuizScore).HasPrecision(18, 2);
            b.Property(x => x.OverallProgress).HasPrecision(5, 2);
            b.Property(x => x.LastUpdatedAt).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new { x.TenantId, x.CourseId, x.StudentId }).IsUnique();
        });

        builder.Entity<ClassProgressView>(b =>
        {
            b.ToTable(ReportingServiceDbProperties.DbTablePrefix + "ClassProgressViews", ReportingServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.LastUpdatedAt).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => new { x.TenantId, x.CourseId }).IsUnique();
        });

        builder.Entity<CourseOutcomeReportView>(b =>
        {
            b.ToTable(ReportingServiceDbProperties.DbTablePrefix + "CourseOutcomeReportViews", ReportingServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.AssignmentScoreSum).HasPrecision(18, 2);
            b.Property(x => x.AvgAssignmentScore).HasPrecision(18, 2);
            b.Property(x => x.QuizScoreSum).HasPrecision(18, 2);
            b.Property(x => x.AvgQuizScore).HasPrecision(18, 2);
            b.Property(x => x.FinalScoreSum).HasPrecision(18, 2);
            b.Property(x => x.FinalScoreAvg).HasPrecision(18, 2);
            b.Property(x => x.CompletionRate).HasPrecision(5, 2);
            b.Property(x => x.PassRate).HasPrecision(5, 2);
            b.Property(x => x.LastUpdatedAt).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => new { x.TenantId, x.CourseId }).IsUnique();
        });

        builder.Entity<TenantSummaryReportView>(b =>
        {
            b.ToTable(ReportingServiceDbProperties.DbTablePrefix + "TenantSummaryReportViews", ReportingServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.LastUpdatedAt).IsRequired();

            b.HasIndex(x => x.TenantId).IsUnique();
        });

    }
}
