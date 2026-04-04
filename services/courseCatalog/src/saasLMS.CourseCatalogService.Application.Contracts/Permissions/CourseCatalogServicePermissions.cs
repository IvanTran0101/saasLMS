using Volo.Abp.Reflection;

namespace saasLMS.CourseCatalogService.Permissions;

public class CourseCatalogServicePermissions
{
    public const string GroupName = "CourseCatalogService";

    public static class Courses
    {
        public const string Default = GroupName + ".Courses";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Publish = Default + ".Publish";
        public const string Hide = Default + ".Hide";
        public const string Reopen = Default + ".Reopen";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
        public const string ListByInstructor = Default + ".ListByInstructor";
        public const string ListByTenant = Default + ".ListByTenant";
        public const string GetOwner = Default + ".GetOwner";
    }

    public static class Chapters
    {
        public const string Default = GroupName + ".Chapters";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
    }

    public static class Lessons
    {
        public const string Default = GroupName + ".Lessons";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
    }

    public static class Materials
    {
        public const string Default = GroupName + ".Materials";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Hide = Default + ".Hide";
        public const string Activate = Default + ".Activate";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(CourseCatalogServicePermissions));
    }
}
