using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.Localization;
using saasLMS.ProductService.Blazor.Menus;
using Volo.Abp.Account.Localization;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Saas.Host.Blazor.Navigation;

namespace saasLMS.Blazor.Client.Navigation;

public class saasLMSMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public saasLMSMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    //This method was intentionally "async" because ABP Suite will generate asnyc method calls here.
    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<saasLMSResource>();
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        context.Menu.AddItem(new ApplicationMenuItem(
            saasLMSMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 0
        ));

        // Hide Home menu item from Student and Instructor (they are redirected to their own dashboard)
        if (currentUser.IsInRole(LmsRoles.Student) || currentUser.IsInRole(LmsRoles.Instructor))
        {
            var homeItem = context.Menu.Items.FirstOrDefault(i => i.Name == saasLMSMenus.Home);
            if (homeItem != null)
                context.Menu.Items.Remove(homeItem);
        }

        // ── Role-based LMS Navigation ──────────────────────────────────────────
        if (currentUser.IsAuthenticated)
        {
            if (currentUser.IsInRole(LmsRoles.Admin))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.AdminDashboard,
                    l["Menu:AdminDashboard"],
                    "/admin/dashboard",
                    icon: "fa fa-tachometer-alt",
                    order: 1
                ));
            }

            if (currentUser.IsInRole(LmsRoles.Instructor))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.InstructorDashboard,
                    l["Menu:InstructorDashboard"],
                    "/instructor/dashboard",
                    icon: "fa fa-chalkboard-teacher",
                    order: 1
                ));

                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.InstructorReport,
                    l["Menu:InstructorReport"],
                    "/instructor/report",
                    icon: "fa fa-chart-bar",
                    order: 2
                ));
            }

            if (currentUser.IsInRole(LmsRoles.Student))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.StudentDashboard,
                    l["Menu:StudentDashboard"],
                    "/student/dashboard",
                    icon: "fa fa-graduation-cap",
                    order: 1
                ));

                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.StudentCourses,
                    l["Menu:StudentCourses"],
                    "/student/courses",
                    icon: "fa fa-th-large",
                    order: 2
                ));

                context.Menu.AddItem(new ApplicationMenuItem(
                    saasLMSMenus.StudentReport,
                    l["Menu:StudentReport"],
                    "/student/report",
                    icon: "fa fa-chart-line",
                    order: 3
                ));
            }

            context.Menu.AddItem(new ApplicationMenuItem(
                saasLMSMenus.Logout,
                l["Menu:Logout"],
                "/logout",
                icon: "fa fa-sign-out-alt",
                order: 999
            ));
        }

        /* Example nested menu definition:

        context.Menu.AddItem(
            new ApplicationMenuItem("Menu0", "Menu Level 0")
            .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
            .AddItem(
                new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                    .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                        .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
            )
            .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
        );

        */

        context.Menu.SetSubItemOrder(ProductServiceMenus.ProductManagement, 1);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        // Hide Identity management from non-admin users (permission granted only for name lookup)
        if (!currentUser.IsInRole(LmsRoles.Admin))
        {
            var identityItem = administration.Items.FirstOrDefault(i => i.Name == IdentityProMenus.GroupName);
            if (identityItem != null)
                administration.Items.Remove(identityItem);
        }

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->Saas
        administration.SetSubItemOrder(SaasHostMenus.GroupName, 2);

        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 4);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 5);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 6);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 7);
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage",
            icon: "fa fa-cog",
            order: 1000,
            target: "_blank").RequireAuthenticated());

        await Task.CompletedTask;
    }
}
