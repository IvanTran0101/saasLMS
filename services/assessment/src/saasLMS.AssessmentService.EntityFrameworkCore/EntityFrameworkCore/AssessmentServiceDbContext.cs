using Microsoft.EntityFrameworkCore;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.AssessmentService.EntityFrameworkCore;

[ConnectionStringName(AssessmentServiceDbProperties.ConnectionStringName)]
public class AssessmentServiceDbContext : AbpDbContext<AssessmentServiceDbContext>
{
    DbSet<Assignment> Assignments { get; set; }
    DbSet<Submission>  Submissions { get; set; }
    DbSet<Quiz>  Quizzes { get; set; }
    DbSet<QuizAttempt>  QuizAttempts { get; set; }

    public AssessmentServiceDbContext(DbContextOptions<AssessmentServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAssessmentService();
    }
}
