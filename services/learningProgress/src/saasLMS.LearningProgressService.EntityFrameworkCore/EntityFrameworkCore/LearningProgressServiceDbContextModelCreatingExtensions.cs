using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseStructures;
using saasLMS.LearningProgressService.Enrollments;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace saasLMS.LearningProgressService.EntityFrameworkCore;

public static class LearningProgressServiceDbContextModelCreatingExtensions
{
    public static void ConfigureLearningProgressService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<CourseProgress>(b =>
        {
            b.ToTable(LearningProgressServiceDbProperties.DbTablePrefix + "CourseProgresses", LearningProgressServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.CompletedLessonsCount).IsRequired();
            b.Property(x => x.TotalLessonsCount).IsRequired();
            b.Property(x => x.ProgressPercent).IsRequired();
            b.Property(x => x.LastAccessedLessonId);
            b.Property(x => x.LastAccessedAt);
            
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new { x.TenantId, x.CourseId, x.StudentId }).IsUnique();
        });
        builder.Entity<LessonProgress>(b =>
        {
            b.ToTable(LearningProgressServiceDbProperties.DbTablePrefix + "LessonProgresses", LearningProgressServiceDbProperties.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.LessonId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.Status).IsRequired();
            
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.LessonId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new { x.TenantId, x.LessonId, x.StudentId }).IsUnique();
        });

        builder.Entity<EnrollmentProjection>(b =>
        {
            b.ToTable(LearningProgressServiceDbProperties.DbTablePrefix + "EnrollmentProjections", LearningProgressServiceDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.EnrollmentId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.EnrolledAt).IsRequired();
            b.Property(x => x.CancelledAt);

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new { x.TenantId, x.CourseId, x.StudentId }).IsUnique();
        });

        builder.Entity<LessonProjection>(b =>
        {
            b.ToTable(LearningProgressServiceDbProperties.DbTablePrefix + "LessonProjections", LearningProgressServiceDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.ChapterId).IsRequired();
            b.Property(x => x.LessonId).IsRequired();
            b.Property(x => x.Title).IsRequired();
            b.Property(x => x.SortOrder).IsRequired();
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.LessonId).IsUnique();
            b.HasIndex(x => new { x.TenantId, x.CourseId });
        });
    }
}
