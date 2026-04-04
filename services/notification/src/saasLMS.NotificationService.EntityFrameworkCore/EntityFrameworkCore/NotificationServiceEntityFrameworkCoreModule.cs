using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace saasLMS.NotificationService.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(NotificationServiceDomainModule)
)]
public class NotificationServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        NotificationServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<NotificationServiceDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<NotificationServiceDbContext>(c =>
            {
                c.UseSqlServer(b =>
                {
                    b.MigrationsHistoryTable("__NotificationService_Migrations");
                });
            });
        });
    }
}
