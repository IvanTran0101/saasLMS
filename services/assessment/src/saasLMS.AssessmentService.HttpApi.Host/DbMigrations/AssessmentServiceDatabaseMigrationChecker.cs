using System;
using Microsoft.Extensions.Logging;
using saasLMS.AssessmentService.EntityFrameworkCore;
using saasLMS.Shared.Hosting.Microservices.DbMigrations.EfCore;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace saasLMS.AssessmentService.DbMigrations;

public class AssessmentServiceDatabaseMigrationChecker : PendingEfCoreMigrationsChecker<AssessmentServiceDbContext>
{
    public AssessmentServiceDatabaseMigrationChecker(
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
            AssessmentServiceDbProperties.ConnectionStringName)
    {

    }
}
