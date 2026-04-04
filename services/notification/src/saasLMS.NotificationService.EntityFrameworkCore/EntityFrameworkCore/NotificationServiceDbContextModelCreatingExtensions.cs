using Microsoft.EntityFrameworkCore;
using saasLMS.NotificationService.Notifications;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace saasLMS.NotificationService.EntityFrameworkCore;

public static class NotificationServiceDbContextModelCreatingExtensions
{
    public static void ConfigureNotificationService(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(NotificationServiceConsts.DbTablePrefix + "YourEntities", NotificationServiceConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Notification>(b =>
        {
            b.ToTable(NotificationServiceDbProperties.DbTablePrefix + "Notifications", NotificationServiceDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Message).IsRequired().HasMaxLength(2000);
            b.Property(x => x.Type).IsRequired();
            b.Property(x => x.IsRead).IsRequired();
            b.Property(x => x.ReferenceType).HasMaxLength(100);
            b.Property(x => x.ReferenceId).HasMaxLength(128);

            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.TenantId, x.UserId, x.IsRead });
        });
        
        
    }
}
