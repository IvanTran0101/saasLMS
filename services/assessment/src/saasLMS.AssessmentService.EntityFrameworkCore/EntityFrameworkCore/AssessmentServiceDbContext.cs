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
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Submission>  Submissions { get; set; }
    public DbSet<Quiz>  Quizzes { get; set; }
    public DbSet<QuizAttempt>  QuizAttempts { get; set; }
    public DbSet<QuizQuestionMap> QuizQuestionMaps { get; set; }

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
