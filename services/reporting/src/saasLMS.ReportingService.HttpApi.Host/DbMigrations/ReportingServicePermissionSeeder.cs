using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using saasLMS.ReportingService.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace saasLMS.ReportingService.DbMigrations;

public class ReportingServicePermissionSeeder : ITransientDependency
{
    private readonly ILogger<ReportingServicePermissionSeeder> _logger;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ReportingServicePermissionSeeder(
        ILogger<ReportingServicePermissionSeeder> logger,
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
            var permissions = new[]
            {
                ReportingServicePermissions.Reports.View,
                ReportingServicePermissions.Reports.StudentView
            };

            _logger.LogInformation("Seeding ReportingService permissions for WebGateway client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "WebGateway",
                permissions,
                tenantId: null);

            _logger.LogInformation("Seeding ReportingService permissions for PublicWebGateway client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "PublicWebGateway",
                permissions,
                tenantId: null);

            _logger.LogInformation("Seeding ReportingService permissions for ReportingService client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "ReportingService",
                permissions,
                tenantId: null);

            _logger.LogInformation("Seeding ReportingService permissions for WebGateway_Swagger client.");
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                "WebGateway_Swagger",
                permissions,
                tenantId: null);

            await uow.CompleteAsync();
        }
    }
}
