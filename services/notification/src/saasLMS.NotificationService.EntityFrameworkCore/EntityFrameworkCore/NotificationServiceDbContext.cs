using Microsoft.EntityFrameworkCore;
using saasLMS.NotificationService.Notifications;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.NotificationService.EntityFrameworkCore;

[ConnectionStringName(NotificationServiceDbProperties.ConnectionStringName)]
public class NotificationServiceDbContext : AbpDbContext<NotificationServiceDbContext>
{
    DbSet<Notification>  Notifications { get; set; }
    public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureNotificationService();
    }
}
