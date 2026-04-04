using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using saasLMS.CourseCatalogService.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace saasLMS.CourseCatalogService.DbMigrations;

public class CourseCatalogServicePermissionSeeder : ITransientDependency
{
    private readonly ILogger<CourseCatalogServicePermissionSeeder> _logger;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public CourseCatalogServicePermissionSeeder(
        ILogger<CourseCatalogServicePermissionSeeder> logger,
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
            _logger.LogInformation("Seeding CourseCatalog permissions for AssessmentService client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "AssessmentService",
                new[] { CourseCatalogServicePermissions.Courses.GetOwner },
                tenantId: null);

            await uow.CompleteAsync();
        }
    }
}
