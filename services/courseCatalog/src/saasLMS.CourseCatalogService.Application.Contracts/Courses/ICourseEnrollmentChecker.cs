using System;
using System.Threading.Tasks;

namespace saasLMS.CourseCatalogService.Courses;

public interface ICourseEnrollmentChecker
{
    Task CheckStudentEnrolledAsync(Guid courseId, Guid? studentId = null);
    Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid? studentId = null);
}
