using Microsoft.EntityFrameworkCore;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.EnrollmentService.EntityFrameworkCore;

[ConnectionStringName(EnrollmentServiceDbProperties.ConnectionStringName)]
public class EnrollmentServiceDbContext : AbpDbContext<EnrollmentServiceDbContext>
{
    public DbSet<Enrollment> Enrollments { get; set; }
    public EnrollmentServiceDbContext(DbContextOptions<EnrollmentServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEnrollmentService();
    }
}
