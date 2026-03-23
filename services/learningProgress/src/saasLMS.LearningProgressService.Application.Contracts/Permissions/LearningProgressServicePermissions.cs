using Volo.Abp.Reflection;

namespace saasLMS.LearningProgressService.Permissions;

public class LearningProgressServicePermissions
{
    public const string GroupName = "LearningProgressService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(LearningProgressServicePermissions));
    }
}
