using Microsoft.EntityFrameworkCore;
using saasLMS.ReportingService.ReadModels;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.ReportingService.EntityFrameworkCore;

[ConnectionStringName(ReportingServiceDbProperties.ConnectionStringName)]
public class ReportingServiceDbContext : AbpDbContext<ReportingServiceDbContext>
{
    public DbSet<StudentCourseProgressView> StudentCourseProgressViews { get; set; }
    public DbSet<ClassProgressView> ClassProgressViews { get; set; }
    public DbSet<CourseOutcomeReportView> CourseOutcomeReportViews { get; set; }
    public DbSet<TenantSummaryReportView> TenantSummaryReportViews { get; set; }

    public ReportingServiceDbContext(DbContextOptions<ReportingServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureReportingService();
    }
}
