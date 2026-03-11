using saasLMS.Blazor.Client;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXTheme.Bundling;
using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.AuditLogging.Blazor.WebAssembly.Bundling;
using Volo.Saas.Host.Blazor.WebAssembly.Bundling;
using Volo.Abp.Account.Pro.Public.Blazor.WebAssembly.Bundling;

namespace saasLMS.Blazor;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXThemeBundlingModule),
    typeof(AbpAccountPublicBlazorWebAssemblyBundlingModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyBundlingModule),
    typeof(SaasHostBlazorWebAssemblyBundlingModule),
    typeof(AbpAspNetCoreMvcUiBundlingModule)
)]
public class saasLMSBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //https://github.com/dotnet/aspnetcore/issues/52530
        Configure<RouteOptions>(options =>
        {
            options.SuppressCheckForUnhandledSecurityMetadata = true;
        });

        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();

        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(WebAppAdditionalAssembliesHelper.GetAssemblies<saasLMSBlazorClientModule>());
        });
    }
}
