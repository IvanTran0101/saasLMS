using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseStructures;
using saasLMS.LearningProgressService.Enrollments;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore;

[ConnectionStringName(LearningProgressServiceDbProperties.ConnectionStringName)]
public class LearningProgressServiceDbContext : AbpDbContext<LearningProgressServiceDbContext>
{
    public DbSet<CourseProgress> CourseProgresses { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<EnrollmentProjection> EnrollmentProjections { get; set; }
    public DbSet<LessonProjection> LessonProjections { get; set; }
    public LearningProgressServiceDbContext(DbContextOptions<LearningProgressServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureLearningProgressService();
    }
}
