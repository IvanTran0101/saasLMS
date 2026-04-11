using System;
using System.Net.Http;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.AdministrationService;
using saasLMS.Blazor.Client.Components.Layout;
using saasLMS.Blazor.Client.Navigation;
using saasLMS.IdentityService;
using saasLMS.ProductService;
using saasLMS.ProductService.Blazor;
using saasLMS.SaasService;
using Volo.Abp.Account.Pro.Admin.Blazor.WebAssembly;
using Volo.Abp.Account.Pro.Public.Blazor.WebAssembly;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AuditLogging.Blazor.WebAssembly;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.Mapperly;
using Volo.Abp.Identity.Pro.Blazor.Server.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using OpenIddict.Abstractions;
using Volo.Abp.AspNetCore.Components.Web.LeptonXTheme.Components;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXTheme;
using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.LanguageManagement.Blazor.WebAssembly;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.OpenIddict.Pro.Blazor.WebAssembly;
using Volo.Abp.TextTemplateManagement.Blazor.WebAssembly;
using Volo.Payment.Admin.Blazor.WebAssembly;
using Volo.Saas.Host;
using Volo.Saas.Host.Blazor.WebAssembly;
using Volo.Saas.Tenant.Blazor.WebAssembly;
using saasLMS.CourseCatalogService;
using saasLMS.CourseCatalogService.Blazor;
using saasLMS.EnrollmentService;
using saasLMS.EnrollmentService.Blazor;

namespace saasLMS.Blazor.Client;

[DependsOn(
    typeof(AbpAutofacWebAssemblyModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXThemeModule),
    typeof(AbpIdentityProBlazorWebAssemblyModule),
    typeof(SaasHostBlazorWebAssemblyModule),
    typeof(SaasTenantBlazorWebAssemblyModule),
    typeof(AbpPaymentAdminBlazorWebAssemblyModule),
    typeof(AbpAccountAdminBlazorWebAssemblyModule),
    typeof(AbpAccountPublicBlazorWebAssemblyModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyModule),
    typeof(TextTemplateManagementBlazorWebAssemblyModule),
    typeof(LanguageManagementBlazorWebAssemblyModule),
    typeof(AbpOpenIddictProBlazorWebAssemblyModule),
    typeof(saasLMSSharedLocalizationModule),
    typeof(ProductServiceBlazorModule),
    typeof(ProductServiceHttpApiClientModule),
    typeof(AdministrationServiceHttpApiClientModule),
    typeof(SaasServiceHttpApiClientModule),
    typeof(IdentityServiceHttpApiClientModule),
    typeof(CourseCatalogServiceBlazorModule),
    typeof(CourseCatalogServiceHttpApiClientModule),
    typeof(EnrollmentServiceBlazorModule),
    typeof(EnrollmentServiceHttpApiClientModule)
)]
public class saasLMSBlazorClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
        var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

        ConfigureAuthentication(builder);
        ConfigureHttpClient(context, environment);
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureMenu(context);
        ConfigureTheme();

        context.Services.AddMapperlyObjectMapper<saasLMSBlazorClientModule>();
    }

    private void ConfigureTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            options.DefaultStyle = LeptonXStyleNames.System;
        });
    }

    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(saasLMSBlazorClientModule).Assembly;
        });
    }

    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new saasLMSMenuContributor(context.Services.GetConfiguration()));
        });
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOidcAuthentication(options =>
        {
            builder.Configuration.Bind("AuthServer", options.ProviderOptions);
            options.UserOptions.NameClaim = OpenIddictConstants.Claims.Name;
            options.UserOptions.RoleClaim = OpenIddictConstants.Claims.Role;
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("email");
            options.ProviderOptions.DefaultScopes.Add("phone");
            options.ProviderOptions.DefaultScopes.Add("AccountService");
            options.ProviderOptions.DefaultScopes.Add("IdentityService");
            options.ProviderOptions.DefaultScopes.Add("AdministrationService");
            options.ProviderOptions.DefaultScopes.Add("SaasService");
            options.ProviderOptions.DefaultScopes.Add("ProductService");
            options.ProviderOptions.DefaultScopes.Add("CourseCatalogService");
            options.ProviderOptions.DefaultScopes.Add("EnrollmentService");
        });
    }

    private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
    {
        context.Services.AddTransient(sp => new HttpClient
        {
            BaseAddress = new Uri(environment.BaseAddress)
        });
    }
}
