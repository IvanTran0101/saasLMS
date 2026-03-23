using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore;

[ConnectionStringName(LearningProgressServiceDbProperties.ConnectionStringName)]
public class LearningProgressServiceDbContext : AbpDbContext<LearningProgressServiceDbContext>
{
    DbSet<CourseProgress>  CourseProgresses { get; set; }
    DbSet<LessonProgress> LessonProgresses { get; set; }
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
