namespace saasLMS.EnrollmentService;

/// Tập trung toàn bộ error code của EnrollmentService.
/// Convention ABP: "{ServiceName}:{ErrorName}"
public static class EnrollmentServiceErrorCodes
{
    public const string TenantNotFound                 = "EnrollmentService:TenantNotFound";
    public const string EmptyCourseId                  = "EnrollmentService:EmptyCourseId";
    public const string CourseNotFound                 = "EnrollmentService:CourseNotFound";
    public const string CourseNotEligibleForEnrollment = "EnrollmentService:CourseNotEligibleForEnrollment";
    public const string CrossTenantAccessDenied        = "EnrollmentService:CrossTenantAccessDenied";
    public const string AlreadyEnrolled                = "EnrollmentService:AlreadyEnrolled";
    public const string NotEnrolled                    = "EnrollmentService:NotEnrolled";
    public const string EnrollmentNotFound             = "EnrollmentService:EnrollmentNotFound";
}