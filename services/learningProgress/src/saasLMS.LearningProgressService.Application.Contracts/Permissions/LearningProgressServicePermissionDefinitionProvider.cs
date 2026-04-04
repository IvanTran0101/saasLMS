using saasLMS.LearningProgressService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.LearningProgressService.Permissions;

public class LearningProgressServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(LearningProgressServicePermissions.GroupName, L("Permission:LearningProgressService"));

        var lessonProgresses = myGroup.AddPermission(
            LearningProgressServicePermissions.LessonProgresses.Default,
            L("Permission:LessonProgresses"));

        lessonProgresses.AddChild(
            LearningProgressServicePermissions.LessonProgresses.Record,
            L("Permission:Record"));

        lessonProgresses.AddChild(
            LearningProgressServicePermissions.LessonProgresses.ViewOwn,
            L("Permission:ViewOwn"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LearningProgressServiceResource>(name);
    }
}
