using System.IO;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp;

namespace saasLMS.EnrollmentService.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 *
 * It is also used in the DbMigrator application.
 * */
public class EnrollmentServiceDbContextFactory : IDesignTimeDbContextFactory<EnrollmentServiceDbContext>
{
    private readonly string _connectionString;

    /* This constructor is used when you use EF Core tooling (e.g. Update-Database) */
    public EnrollmentServiceDbContextFactory()
    {
        _connectionString = GetConnectionStringFromConfiguration();
    }

    /* This constructor is used by DbMigrator application */
    public EnrollmentServiceDbContextFactory([NotNull] string connectionString)
    {
        Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
        _connectionString = connectionString;
    }

    public EnrollmentServiceDbContext CreateDbContext(string[] args)
    {
        EnrollmentServiceEfCoreEntityExtensionMappings.Configure();
        
        var builder = new DbContextOptionsBuilder<EnrollmentServiceDbContext>()
            .UseSqlServer(_connectionString, b =>
            {
                b.MigrationsHistoryTable("__EnrollmentService_Migrations");
            });

        return new EnrollmentServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration()
            .GetConnectionString(EnrollmentServiceDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    $"..{Path.DirectorySeparatorChar}saasLMS.EnrollmentService.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
