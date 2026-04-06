using System;
using System.Threading.Tasks;

namespace saasLMS.CourseCatalogService.Courses;

public interface ICourseAccessChecker
{
    Task CheckCanManageCourseAsync(Guid courseId, Guid? userId = null);
    Task<bool> CanManageCourseAsync(Guid courseId, Guid? userId = null);
}
