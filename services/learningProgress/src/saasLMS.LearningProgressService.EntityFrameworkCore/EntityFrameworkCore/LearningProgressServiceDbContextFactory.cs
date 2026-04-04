using System.IO;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Volo.Abp;

namespace saasLMS.LearningProgressService.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 *
 * It is also used in the DbMigrator application.
 * */
public class LearningProgressServiceDbContextFactory : IDesignTimeDbContextFactory<LearningProgressServiceDbContext>
{
    private readonly string _connectionString;

    /* This constructor is used when you use EF Core tooling (e.g. Update-Database) */
    public LearningProgressServiceDbContextFactory()
    {
        _connectionString = GetConnectionStringFromConfiguration();
    }

    /* This constructor is used by DbMigrator application */
    public LearningProgressServiceDbContextFactory([NotNull] string connectionString)
    {
        Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
        _connectionString = connectionString;
    }

    public LearningProgressServiceDbContext CreateDbContext(string[] args)
    {
        LearningProgressServiceEfCoreEntityExtensionMappings.Configure();
        
        var builder = new DbContextOptionsBuilder<LearningProgressServiceDbContext>()
            .UseSqlServer(_connectionString, b =>
            {
                b.MigrationsHistoryTable("__LearningProgressService_Migrations");
            });

        return new LearningProgressServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration()
            .GetConnectionString(LearningProgressServiceDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    $"..{Path.DirectorySeparatorChar}saasLMS.LearningProgressService.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
