using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Users;

namespace saasLMS.EnrollmentService;

public class EnrollmentAppService : EnrollmentServiceAppService, IEnrollmentAppService  
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly EnrollmentManager _enrollmentManager;
 
    public EnrollmentAppService(
        IEnrollmentRepository enrollmentRepository,
        EnrollmentManager enrollmentManager)
    {
        _enrollmentRepository = enrollmentRepository;
        _enrollmentManager = enrollmentManager;
    }
    
    public async Task<EnrollmentDto> EnrollAsync(EnrollCourseInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("EnrollmentService:EmptyCourseId");
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollment = await _enrollmentManager.CreateAsync(
            tenantId.Value,
            input.CourseId,
            studentId,
            Clock.Now);
 
        await _enrollmentRepository.InsertAsync(enrollment, autoSave: true);
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    public async Task<EnrollmentDto> CancelAsync(CancelEnrollmentInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("EnrollmentService:EmptyCourseId");
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollment = await _enrollmentRepository.FindByCourseAndStudentAsync(
            tenantId.Value,
            input.CourseId,
            studentId);
 
        if (enrollment == null)
        {
            throw new BusinessException("EnrollmentService:NotEnrolled");
        }
 
        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            throw new BusinessException("EnrollmentService:NotEnrolled");
        }
 
        await _enrollmentManager.CancelAsync(enrollment, Clock.Now);
        await _enrollmentRepository.UpdateAsync(enrollment, autoSave: true);
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    public async Task<EnrollmentDto> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("EnrollmentService:EnrollmentNotFound");
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        var enrollment = await _enrollmentRepository.GetAsync(id);
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    public async Task<EnrollmentDto?> FindByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("EnrollmentService:EmptyCourseId");
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollment = await _enrollmentRepository.FindByCourseAndStudentAsync(
            tenantId.Value,
            courseId,
            studentId);
 
        return enrollment == null
            ? null
            : ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    public async Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync()
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollments = await _enrollmentRepository.GetListByStudentAsync(
            tenantId.Value,
            studentId);
 
        return ObjectMapper.Map<List<Enrollment>, List<EnrollmentListItemDto>>(enrollments);
    }
    
    public async Task<List<EnrollmentListItemDto>> GetEnrollmentsByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("EnrollmentService:EmptyCourseId");
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("EnrollmentService:TenantNotFound");
        }
 
        var enrollments = await _enrollmentRepository.GetListByCourseAsync(
            tenantId.Value,
            courseId);
 
        return ObjectMapper.Map<List<Enrollment>, List<EnrollmentListItemDto>>(enrollments);
    }
}