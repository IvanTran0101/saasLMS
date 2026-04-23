using saasLMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace saasLMS.AssessmentService.Permissions;

public class AssessmentServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AssessmentServicePermissions.GroupName, L("Permission:AssessmentService"));

        var quizzes = myGroup.AddPermission(AssessmentServicePermissions.Quizzes.Default, L("Permission:Quizzes"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.Create, L("Permission:Create"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.Update, L("Permission:Update"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.Publish, L("Permission:Publish"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.Close, L("Permission:Close"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.View, L("Permission:View"));
        quizzes.AddChild(AssessmentServicePermissions.Quizzes.ViewPublished, L("Permission:ViewPublished"));

        var assignments = myGroup.AddPermission(AssessmentServicePermissions.Assignments.Default, L("Permission:Assignments"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.Create, L("Permission:Create"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.Update, L("Permission:Update"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.Publish, L("Permission:Publish"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.Close, L("Permission:Close"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.View, L("Permission:View"));
        assignments.AddChild(AssessmentServicePermissions.Assignments.ViewPublished, L("Permission:ViewPublished"));

        var submissions = myGroup.AddPermission(AssessmentServicePermissions.Submissions.Default, L("Permission:Submissions"));
        submissions.AddChild(AssessmentServicePermissions.Submissions.Submit, L("Permission:Submit"));
        submissions.AddChild(AssessmentServicePermissions.Submissions.View, L("Permission:View"));
        submissions.AddChild(AssessmentServicePermissions.Submissions.Grade, L("Permission:Grade"));
        submissions.AddChild(AssessmentServicePermissions.Submissions.ViewOwn, L("Permission:ViewOwn"));

        var quizAttempts = myGroup.AddPermission(AssessmentServicePermissions.QuizAttempts.Default, L("Permission:QuizAttempts"));
        quizAttempts.AddChild(AssessmentServicePermissions.QuizAttempts.Start, L("Permission:Start"));
        quizAttempts.AddChild(AssessmentServicePermissions.QuizAttempts.Submit, L("Permission:Submit"));
        quizAttempts.AddChild(AssessmentServicePermissions.QuizAttempts.View, L("Permission:View"));
        quizAttempts.AddChild(AssessmentServicePermissions.QuizAttempts.ViewOwn, L("Permission:ViewOwn"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<saasLMSResource>(name);
    }
}
