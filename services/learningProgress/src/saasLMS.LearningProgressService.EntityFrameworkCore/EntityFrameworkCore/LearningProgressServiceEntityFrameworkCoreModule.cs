using Microsoft.Extensions.DependencyInjection;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseStructures;
using saasLMS.LearningProgressService.Enrollments;
using saasLMS.LearningProgressService.EntityFrameworkCore.CourseProgresses;
using saasLMS.LearningProgressService.EntityFrameworkCore.CourseStructures;
using saasLMS.LearningProgressService.EntityFrameworkCore.Enrollments;
using saasLMS.LearningProgressService.EntityFrameworkCore.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace saasLMS.LearningProgressService.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(LearningProgressServiceDomainModule)
)]
public class LearningProgressServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        LearningProgressServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<LearningProgressServiceDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<LessonProgress, LessonProgressRepository>();
            options.AddRepository<CourseProgress, CourseProgressRepository>();
            options.AddRepository<EnrollmentProjection, EnrollmentProjectionRepository>();
            options.AddRepository<LessonProjection, LessonProjectionRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<LearningProgressServiceDbContext>(c =>
            {
                c.UseSqlServer(b =>
                {
                    b.MigrationsHistoryTable("__LearningProgressService_Migrations");
                });
            });
        });
    }
}
