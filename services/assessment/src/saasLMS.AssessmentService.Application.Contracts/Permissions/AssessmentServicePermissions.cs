using Volo.Abp.Reflection;

namespace saasLMS.AssessmentService.Permissions;

public class AssessmentServicePermissions
{
    public const string GroupName = "AssessmentService";

    public static class Quizzes
    {
        public const string Default = GroupName + ".Quizzes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Publish = Default + ".Publish";
        public const string Close = Default + ".Close";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
    }

    public static class Assignments
    {
        public const string Default = GroupName + ".Assignments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Publish = Default + ".Publish";
        public const string Close = Default + ".Close";
        public const string View = Default + ".View";
        public const string ViewPublished = Default + ".ViewPublished";
    }

    public static class Submissions
    {
        public const string Default = GroupName + ".Submissions";
        public const string Submit = Default + ".Submit";
        public const string View = Default + ".View";
        public const string Grade = Default + ".Grade";
        public const string ViewOwn = Default + ".ViewOwn";
    }

    public static class QuizAttempts
    {
        public const string Default = GroupName + ".QuizAttempts";
        public const string Start = Default + ".Start";
        public const string Submit = Default + ".Submit";
        public const string View = Default + ".View";
        public const string ViewOwn = Default + ".ViewOwn";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AssessmentServicePermissions));
    }
}
