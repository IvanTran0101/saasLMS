using saasLMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.CourseCatalogService.Permissions;

public class CourseCatalogServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CourseCatalogServicePermissions.GroupName, L("Permission:CourseCatalogService"));

        var courses = myGroup.AddPermission(CourseCatalogServicePermissions.Courses.Default, L("Permission:Courses"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.Create, L("Permission:Create"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.Update, L("Permission:Update"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.Publish, L("Permission:Publish"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.Hide, L("Permission:Hide"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.Reopen, L("Permission:Reopen"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.View, L("Permission:View"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.ViewPublished, L("Permission:ViewPublished"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.ListByInstructor, L("Permission:ListByInstructor"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.ListByTenant, L("Permission:ListByTenant"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.GetOwner, L("Permission:GetOwner"));
        courses.AddChild(CourseCatalogServicePermissions.Courses.CheckEligibility, L("Permission:CheckEligibility"));

        var chapters = myGroup.AddPermission(CourseCatalogServicePermissions.Chapters.Default, L("Permission:Chapters"));
        chapters.AddChild(CourseCatalogServicePermissions.Chapters.Create, L("Permission:Create"));
        chapters.AddChild(CourseCatalogServicePermissions.Chapters.Update, L("Permission:Update"));
        chapters.AddChild(CourseCatalogServicePermissions.Chapters.Delete, L("Permission:Delete"));
        chapters.AddChild(CourseCatalogServicePermissions.Chapters.View, L("Permission:View"));
        chapters.AddChild(CourseCatalogServicePermissions.Chapters.ViewPublished, L("Permission:ViewPublished"));

        var lessons = myGroup.AddPermission(CourseCatalogServicePermissions.Lessons.Default, L("Permission:Lessons"));
        lessons.AddChild(CourseCatalogServicePermissions.Lessons.Create, L("Permission:Create"));
        lessons.AddChild(CourseCatalogServicePermissions.Lessons.Update, L("Permission:Update"));
        lessons.AddChild(CourseCatalogServicePermissions.Lessons.Delete, L("Permission:Delete"));
        lessons.AddChild(CourseCatalogServicePermissions.Lessons.View, L("Permission:View"));
        lessons.AddChild(CourseCatalogServicePermissions.Lessons.ViewPublished, L("Permission:ViewPublished"));

        var materials = myGroup.AddPermission(CourseCatalogServicePermissions.Materials.Default, L("Permission:Materials"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.Create, L("Permission:Create"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.Update, L("Permission:Update"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.Delete, L("Permission:Delete"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.Hide, L("Permission:Hide"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.Activate, L("Permission:Activate"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.View, L("Permission:View"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.ViewPublished, L("Permission:ViewPublished"));
        materials.AddChild(CourseCatalogServicePermissions.Materials.DownloadPublished, L("Permission:DownloadPublished"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<saasLMSResource>(name);
    }
}
