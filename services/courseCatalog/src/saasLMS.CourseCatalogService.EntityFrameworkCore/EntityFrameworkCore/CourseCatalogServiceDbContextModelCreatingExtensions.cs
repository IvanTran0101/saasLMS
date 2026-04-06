using Microsoft.EntityFrameworkCore;
using saasLMS.CourseCatalogService.Courses;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace saasLMS.CourseCatalogService.EntityFrameworkCore;

public static class CourseCatalogServiceDbContextModelCreatingExtensions
{
    public static void ConfigureCourseCatalogService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(CourseCatalogServiceConsts.DbTablePrefix + "YourEntities", CourseCatalogServiceConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Course>(b =>
        {
            b.ToTable(CourseCatalogServiceConsts.DbTablePrefix + "Courses",
                CourseCatalogServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x =>
                x.TenantId).IsRequired();
            b.Property(x =>
                x.Title).IsRequired().HasMaxLength(200);
            b.Property(x =>
                x.Description).HasMaxLength(2000);
            b.Property(x =>
                x.Status).IsRequired();
            b.Property(x =>
                x.InstructorId).IsRequired();
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.InstructorId);
            b.HasIndex(x => new { x.TenantId, x.Title }).IsUnique();
            b.HasMany(x => x.Chapters)
                .WithOne()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Chapters).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
        builder.Entity<Chapter>(b =>
        {
            b.ToTable(CourseCatalogServiceConsts.DbTablePrefix + "Chapters",
                CourseCatalogServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x =>
                x.CourseId).IsRequired();
            b.Property(x =>
                x.Title).IsRequired().HasMaxLength(200);
            b.Property(x =>
                x.OrderNo).IsRequired();
            b.HasIndex(x => new { x.CourseId, x.OrderNo }).IsUnique();
            b.HasMany(x => x.Lessons)
                .WithOne()
                .HasForeignKey(x => x.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Lessons).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
        builder.Entity<Lesson>(b =>
        {
            b.ToTable(CourseCatalogServiceConsts.DbTablePrefix + "Lessons",
                CourseCatalogServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x =>
                x.ChapterId).IsRequired();
            b.Property(x =>
                x.Title).IsRequired().HasMaxLength(200);
            b.Property(x =>
                x.SortOrder).IsRequired();
            b.Property(x =>
                x.ContentState).IsRequired();
            b.HasIndex(x => new { x.ChapterId, x.SortOrder }).IsUnique();
            b.HasMany(x => x.Materials)
                .WithOne()
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(x => x.Materials).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
        builder.Entity<Material>(b =>
        {
            b.ToTable(CourseCatalogServiceConsts.DbTablePrefix + "Materials",
                CourseCatalogServiceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();

            b.Property(x =>
                x.LessonId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x =>
                x.Type).IsRequired();
            b.Property(x =>
                x.StorageKey).HasMaxLength(512);
            b.Property(x =>
                x.FileName).HasMaxLength(512);
            b.Property(x =>
                x.MimeType).HasMaxLength(128);
            b.Property(x =>
                x.ExternalUrl).HasMaxLength(1000);
            b.Property(x =>
                x.Status).IsRequired();
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.LessonId);
            b.HasIndex(x => new { x.LessonId, x.Type });
        });
    }
}
