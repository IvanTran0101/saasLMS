using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Submissions;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.EntityFrameworkCore.Modeling;
namespace saasLMS.AssessmentService.EntityFrameworkCore;

public static class AssessmentServiceDbContextModelCreatingExtensions
{
    public static void ConfigureAssessmentService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(AssessmentServiceConsts.DbTablePrefix + "YourEntities", AssessmentServiceConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Assignment>(b =>
        {
            b.ToTable(AssessmentServiceConsts.DbTablePrefix + "Assignment",
                AssessmentServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.MaxScore).IsRequired();
            b.Property(x => x.Status).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.LessonId);
        });
        builder.Entity<Submission>(b =>
        {
            b.ToTable(AssessmentServiceConsts.DbTablePrefix + "Submission",
                AssessmentServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.AssignmentId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.SubmittedAt).IsRequired();
            b.Property(x => x.ContentType).IsRequired();
            b.Property(x => x.ContentRef).IsRequired().HasMaxLength(1000);
            b.Property(x => x.FileName).HasMaxLength(255);
            b.Property(x => x.MimeType).HasMaxLength(128);
            b.Property(x => x.Status).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.AssignmentId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new {x.TenantId, x.AssignmentId, x.StudentId}).IsUnique();
            
            b.HasOne<Assignment>()
                .WithMany()
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Quiz>(b =>
        {
            b.ToTable(AssessmentServiceConsts.DbTablePrefix + "Quizzes", AssessmentServiceConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.LessonId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.MaxScore).IsRequired();
            b.Property(x => x.AttemptPolicy).IsRequired();
            b.Property(x => x.QuestionsJson).IsRequired().HasColumnType("text");
            b.Property(x => x.Status).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.LessonId);
        });
        builder.Entity<QuizAttempt>(b =>
        {
            b.ToTable(AssessmentServiceConsts.DbTablePrefix + "QuizAttempts", AssessmentServiceConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.QuizId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x => x.AttemptNumber).IsRequired();
            b.Property(x => x.StartedAt).IsRequired();
            b.Property(x => x.Status).IsRequired();

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.QuizId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new { x.TenantId, x.QuizId, x.StudentId, x.AttemptNumber }).IsUnique();

            b.HasOne<Quiz>()
                .WithMany()
                .HasForeignKey(x => x.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
    }
}
