namespace saasLMS.Blazor.Client.Authorization;

/// <summary>
/// Role name constants for the LMS application.
/// These must match exactly (case-sensitive) the role names seeded in ABP Identity.
/// Adjust the values here if the actual role names differ.
/// </summary>
public static class LmsRoles
{
    public const string Admin      = "admin";
    public const string Instructor = "Instructor";
    public const string Student    = "Student";
}
