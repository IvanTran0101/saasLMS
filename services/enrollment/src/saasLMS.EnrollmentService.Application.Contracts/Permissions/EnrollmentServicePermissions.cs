using Volo.Abp.Reflection;

namespace saasLMS.EnrollmentService.Permissions;

public class EnrollmentServicePermissions
{
    public const string GroupName = "EnrollmentService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(EnrollmentServicePermissions));
    }
}
