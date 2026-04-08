using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using saasLMS.EnrollmentService.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace saasLMS.EnrollmentService.DbMigrations;

public class EnrollmentServicePermissionSeeder : ITransientDependency
{
    private readonly ILogger<EnrollmentServicePermissionSeeder> _logger;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public EnrollmentServicePermissionSeeder(
        ILogger<EnrollmentServicePermissionSeeder> logger,
        IPermissionDataSeeder permissionDataSeeder,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _logger = logger;
        _permissionDataSeeder = permissionDataSeeder;
        _currentTenant = currentTenant;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            _logger.LogInformation("Seeding EnrollmentService permissions for CourseCatalogService client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "CourseCatalogService",
                new[] { EnrollmentServicePermissions.Enrollments.CheckActive },
                tenantId: null);
            
            _logger.LogInformation("Seeding EnrollmentService permissions for LearningProgressService client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "LearningProgressService",
                new[] { EnrollmentServicePermissions.Enrollments.CheckActive },
                tenantId: null);

            await uow.CompleteAsync();
        }
    }
}
