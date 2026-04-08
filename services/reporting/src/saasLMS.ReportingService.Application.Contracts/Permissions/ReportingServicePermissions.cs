using Volo.Abp.Reflection;

namespace saasLMS.ReportingService.Permissions;

public class ReportingServicePermissions
{
    public const string GroupName = "ReportingService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ReportingServicePermissions));
    }
}
