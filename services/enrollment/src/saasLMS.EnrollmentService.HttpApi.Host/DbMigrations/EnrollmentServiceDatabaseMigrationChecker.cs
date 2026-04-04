using System;
using Microsoft.Extensions.Logging;
using saasLMS.EnrollmentService.EntityFrameworkCore;
using saasLMS.Shared.Hosting.Microservices.DbMigrations.EfCore;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace saasLMS.EnrollmentService.DbMigrations;

public class EnrollmentServiceDatabaseMigrationChecker : PendingEfCoreMigrationsChecker<EnrollmentServiceDbContext>
{
    public EnrollmentServiceDatabaseMigrationChecker(
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
            EnrollmentServiceDbProperties.ConnectionStringName)
    {

    }
}
