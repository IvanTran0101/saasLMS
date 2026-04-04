using Volo.Abp.Reflection;

namespace saasLMS.CourseCatalogService.Permissions;

public class CourseCatalogServicePermissions
{
    public const string GroupName = "CourseCatalogService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(CourseCatalogServicePermissions));
    }
}
