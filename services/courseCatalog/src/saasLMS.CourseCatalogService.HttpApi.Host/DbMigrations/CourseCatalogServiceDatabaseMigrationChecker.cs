using System;
using Microsoft.Extensions.Logging;
using saasLMS.CourseCatalogService.EntityFrameworkCore;
using saasLMS.Shared.Hosting.Microservices.DbMigrations.EfCore;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace saasLMS.CourseCatalogService.DbMigrations;

public class CourseCatalogServiceDatabaseMigrationChecker : PendingEfCoreMigrationsChecker<CourseCatalogServiceDbContext>
{
    public CourseCatalogServiceDatabaseMigrationChecker(
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IDistributedEventBus distributedEventBus,
        IAbpDistributedLock abpDistributedLock)
        : base(
            loggerFactory,
            unitOfWorkManager,
            serviceProvider,
            currentTenant,
            distributedEventBus,
            abpDistributedLock,
            CourseCatalogServiceDbProperties.ConnectionStringName)
    {

    }
}
