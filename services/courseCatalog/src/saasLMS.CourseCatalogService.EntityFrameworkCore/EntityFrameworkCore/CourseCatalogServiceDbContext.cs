using Microsoft.EntityFrameworkCore;
using saasLMS.CourseCatalogService.Courses;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.CourseCatalogService.EntityFrameworkCore;

[ConnectionStringName(CourseCatalogServiceDbProperties.ConnectionStringName)]
public class CourseCatalogServiceDbContext : AbpDbContext<CourseCatalogServiceDbContext>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Material> Materials { get; set; }
    public CourseCatalogServiceDbContext(DbContextOptions<CourseCatalogServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureCourseCatalogService();
    }
}
