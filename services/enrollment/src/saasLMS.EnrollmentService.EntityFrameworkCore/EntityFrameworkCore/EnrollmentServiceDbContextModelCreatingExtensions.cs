using Microsoft.EntityFrameworkCore;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp;

namespace saasLMS.EnrollmentService.EntityFrameworkCore;

public static class EnrollmentServiceDbContextModelCreatingExtensions
{
    public static void ConfigureEnrollmentService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(EnrollmentServiceConsts.DbTablePrefix + "YourEntities", EnrollmentServiceConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Enrollment>(b =>
        {
            b.ToTable(EnrollmentServiceConsts.DbTablePrefix + "Enrollment", EnrollmentServiceConsts.DbSchema);
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.CourseId).IsRequired();
            b.Property(x => x.StudentId).IsRequired();
            b.Property(x=> x.Status).IsRequired();
            b.Property(x=> x.EnrolledAt).IsRequired();
            b.Property(x => x.CompletedAt);
            b.Property(x => x.CancelledAt);

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.CourseId);
            b.HasIndex(x => x.StudentId);
            b.HasIndex(x => new {x.TenantId, x.CourseId, x.StudentId}).IsUnique();
        });
    }
}
