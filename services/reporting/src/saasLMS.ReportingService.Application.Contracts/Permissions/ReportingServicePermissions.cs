using Volo.Abp.Reflection;

namespace saasLMS.ReportingService.Permissions;

public class ReportingServicePermissions
{
    public const string GroupName = "ReportingService";

    public static class Reports
    {
        public const string View = GroupName + ".Reports.View";
        public const string StudentView = GroupName + ".Reports.StudentView";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(ReportingServicePermissions));
    }
}
