using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace saasLMS.AuthServer;

public class AuthServerOpenIddictDataSeeder : IDataSeedContributor, ITransientDependency
{
    private static readonly string[] ServiceScopes =
    [
        "AccountService",
        "IdentityService",
        "AdministrationService",
        "SaasService",
        "ProductService",
        "AssessmentService",
        "CourseCatalogService",
        "EnrollmentService",
        "LearningProgressService",
        "NotificationService"
    ];

    private readonly ICurrentTenant _currentTenant;
    private readonly IOpenIddictApplicationRepository _applicationRepository;
    private readonly IAbpApplicationManager _applicationManager;
    private readonly IOpenIddictScopeRepository _scopeRepository;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthServerOpenIddictDataSeeder(
        ICurrentTenant currentTenant,
        IOpenIddictApplicationRepository applicationRepository,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeRepository scopeRepository,
        IOpenIddictScopeManager scopeManager)
    {
        _currentTenant = currentTenant;
        _applicationRepository = applicationRepository;
        _applicationManager = applicationManager;
        _scopeRepository = scopeRepository;
        _scopeManager = scopeManager;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        return SeedAsync();
    }

    [UnitOfWork]
    public virtual async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        {
            await CreateApiScopesAsync();
            await CreateServiceClientsAsync();
            await CreateSwaggerUiClientAsync();
        }
    }

    private async Task CreateApiScopesAsync()
    {
        foreach (var scope in ServiceScopes)
        {
            if (await _scopeRepository.FindByNameAsync(scope) != null)
            {
                continue;
            }

            await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = scope,
                DisplayName = scope + " API",
                Resources = { scope }
            });
        }
    }

    private async Task CreateServiceClientsAsync()
    {
        var permissions = new List<string>
        {
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
        };

        permissions.AddRange(ServiceScopes.Select(s => OpenIddictConstants.Permissions.Prefixes.Scope + s));

        foreach (var clientId in ServiceScopes)
        {
            var client = await _applicationRepository.FindByClientIdAsync(clientId);
            var descriptor = new AbpApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = "1q2w3e*",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                DisplayName = clientId,
                ClientType = OpenIddictConstants.ClientTypes.Confidential
            };

            foreach (var permission in permissions)
            {
                descriptor.Permissions.Add(permission);
            }

            if (client == null)
            {
                await _applicationManager.CreateAsync(descriptor);
            }
            else
            {
                await _applicationManager.UpdateAsync(client.ToModel(), descriptor);
            }
        }
    }

    private async Task CreateSwaggerUiClientAsync()
    {
        const string clientId = "WebGateway_Swagger";

        var client = await _applicationRepository.FindByClientIdAsync(clientId);
        var descriptor = new AbpApplicationDescriptor
        {
            ClientId = clientId,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
            DisplayName = "WebGateway Swagger UI"
        };

        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);

        foreach (var scope in ServiceScopes)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
        }

        var baseUrls = new[]
        {
            "https://localhost:44325",
            "https://localhost:44353",
            "https://localhost:44367",
            "https://localhost:44388",
            "https://localhost:44381",
            "https://localhost:44361",
            "https://localhost:45209",
            "https://localhost:44445",
            "https://localhost:44854",
            "https://localhost:45256",
            "https://localhost:44664"
        };

        foreach (var baseUrl in baseUrls)
        {
            descriptor.RedirectUris.Add(new Uri($"{baseUrl}/swagger/oauth2-redirect.html"));
            descriptor.RedirectUris.Add(new Uri($"{baseUrl}/swagger/ui/oauth2-redirect.html"));
        }

        if (client == null)
        {
            await _applicationManager.CreateAsync(descriptor);
        }
        else
        {
            await _applicationManager.UpdateAsync(client.ToModel(), descriptor);
        }
    }
}
