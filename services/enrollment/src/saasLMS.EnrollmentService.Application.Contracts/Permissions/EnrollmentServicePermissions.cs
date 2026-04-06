using Volo.Abp.Reflection;

namespace saasLMS.EnrollmentService.Permissions;

public class EnrollmentServicePermissions
{
    public const string GroupName = "EnrollmentService";
    
    public static class Enrollments
    {
        public const string Default = GroupName + ".Enrollments";
        public const string Create  = Default + ".Create";   // Enroll
        public const string Cancel  = Default + ".Cancel";   // Unenroll
        public const string View    = Default + ".View";     // Admin/Instructor xem tất cả
        public const string ViewOwn = Default + ".ViewOwn";  // Student xem của mình
    }

    public static class Enrollments
    {
        public const string Default = GroupName + ".Enrollments";
        public const string Create = Default + ".Create";
        public const string Cancel = Default + ".Cancel";
        public const string View = Default + ".View";
        public const string ViewMy = Default + ".ViewMy";
        public const string ViewByCourse = Default + ".ViewByCourse";
        public const string CheckActive = Default + ".CheckActive";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(EnrollmentServicePermissions));
    }
}
