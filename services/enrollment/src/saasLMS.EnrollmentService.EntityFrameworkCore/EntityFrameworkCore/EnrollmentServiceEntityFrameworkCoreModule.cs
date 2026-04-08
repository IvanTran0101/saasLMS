using Microsoft.Extensions.DependencyInjection;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.EntityFrameworkCore.Enrollments;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace saasLMS.EnrollmentService.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(EnrollmentServiceDomainModule)
)]
public class EnrollmentServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        EnrollmentServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<EnrollmentServiceDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<Enrollment, EnrollmentRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<EnrollmentServiceDbContext>(c =>
            {
                c.UseSqlServer(b =>
                {
                    b.MigrationsHistoryTable("__EnrollmentService_Migrations");
                });
            });
        });
    }
}
