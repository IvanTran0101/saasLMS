using Volo.Abp.Reflection;

namespace saasLMS.AssessmentService.Permissions;

public class AssessmentServicePermissions
{
    public const string GroupName = "AssessmentService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AssessmentServicePermissions));
    }
}
