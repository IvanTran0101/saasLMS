using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.EnrollmentService.Enrollments;

public interface IEnrollmentRepository : IRepository<Enrollment, Guid>
{
    Task<Enrollment?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    
    Task<List<Enrollment>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        EnrollmentStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<List<Enrollment>> GetListByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default
    );
    
    // Kiểm tra sinh viên có enrollment ACTIVE hay không.
    // Chỉ check status = Active 
    // Sinh viên đã Cancelled trước đó vẫn được phép re-enroll.
    Task<bool> ExistsActiveAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}