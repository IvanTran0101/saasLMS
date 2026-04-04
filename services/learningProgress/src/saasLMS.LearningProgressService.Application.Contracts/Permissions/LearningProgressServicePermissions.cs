using Volo.Abp.Reflection;

namespace saasLMS.LearningProgressService.Permissions;

public class LearningProgressServicePermissions
{
    public const string GroupName = "LearningProgressService";
    
    public static class LessonProgresses
    {
        public const string Default = GroupName + ".LessonProgresses";
        public const string Record  = Default + ".Record";   
        public const string ViewOwn = Default + ".ViewOwn";  
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(LearningProgressServicePermissions));
    }
}
