using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using saasLMS.EnrollmentService.Permissions;
using Volo.Abp;
using Volo.Abp.Users;

namespace saasLMS.EnrollmentService;

public class EnrollmentAppService : EnrollmentServiceAppService, IEnrollmentAppService  
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly EnrollmentManager _enrollmentManager;
    private readonly ICourseCatalogGateway _courseCatalogGateway;
 
    public EnrollmentAppService(
        IEnrollmentRepository enrollmentRepository,
        EnrollmentManager enrollmentManager,
        ICourseCatalogGateway courseCatalogGateway)
    {
        _enrollmentRepository = enrollmentRepository;
        _enrollmentManager = enrollmentManager;
        _courseCatalogGateway = courseCatalogGateway;
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.Create)]
    public async Task<EnrollmentDto> EnrollAsync(EnrollCourseInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var tenantId = CurrentTenant.Id
                       ?? throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);

 
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.EmptyCourseId);
        }
        
        await ValidateCourseEligibilityAsync(input.CourseId, tenantId);
        var studentId = CurrentUser.GetId();
 
        var enrollment = await _enrollmentManager.CreateAsync(
            tenantId,
            input.CourseId,
            studentId,
            Clock.Now);
 
        await _enrollmentRepository.InsertAsync(enrollment, autoSave: true);
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.Cancel)]
    public async Task<EnrollmentDto> CancelAsync(CancelEnrollmentInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
        }
 
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.EmptyCourseId);
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollment = await _enrollmentRepository.FindByCourseAndStudentAsync(
            tenantId.Value,
            input.CourseId,
            studentId);
 
        if (enrollment == null || enrollment.Status == EnrollmentStatus.Cancelled)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.NotEnrolled)
                .WithData("CourseId", input.CourseId);
        }
 
        await _enrollmentManager.CancelAsync(enrollment, Clock.Now);
        await _enrollmentRepository.UpdateAsync(enrollment, autoSave: true);
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.ViewOwn)]
    public async Task<EnrollmentDto> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.EnrollmentNotFound);
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
        }
 
        var enrollment = await _enrollmentRepository.GetAsync(id);
        
        if (enrollment.TenantId != tenantId)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.CrossTenantAccessDenied)
                .WithData("EnrollmentId", id);
        }
 
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.ViewOwn)]
    public async Task<EnrollmentDto?> FindByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.EmptyCourseId);
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
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
    
    [Authorize(EnrollmentServicePermissions.Enrollments.ViewOwn)]
    public async Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync(GetMyEnrollmentsInput input)
    {
        Check.NotNull(input, nameof(input));
        
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
        }
 
        var studentId = CurrentUser.GetId();
 
        var enrollments = await _enrollmentRepository.GetListByStudentAsync(
            tenantId.Value,
            studentId,
            status: input.Status);
 
        return ObjectMapper.Map<List<Enrollment>, List<EnrollmentListItemDto>>(enrollments);
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.View)]
    public async Task<List<EnrollmentListItemDto>> GetEnrollmentsByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.EmptyCourseId);
        }
 
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
        }
 
        var enrollments = await _enrollmentRepository.GetListByCourseAsync(
            tenantId.Value,
            courseId);
 
        return ObjectMapper.Map<List<Enrollment>, List<EnrollmentListItemDto>>(enrollments);
    }
    
    [Authorize(EnrollmentServicePermissions.Enrollments.View)]
    public async Task<ActiveEnrollmentDto> CheckActiveEnrollmentAsync(Guid courseId, Guid studentId)
    {
        if (courseId == Guid.Empty || studentId == Guid.Empty)
        {
            return new ActiveEnrollmentDto { IsActive = false };
        }
 
        var tenantId = CurrentTenant.Id
                       ?? throw new BusinessException(EnrollmentServiceErrorCodes.TenantNotFound);
 
        var enrollment = await _enrollmentRepository.FindByCourseAndStudentAsync(
            tenantId,
            courseId,
            studentId);
 
        var isActive = enrollment?.Status == EnrollmentStatus.Active;
 
        return new ActiveEnrollmentDto
        {
            IsActive     = isActive,
            EnrollmentId = isActive ? enrollment!.Id         : null,
            EnrolledAt   = isActive ? enrollment!.EnrolledAt : null
        };
    }
    
    //Private Helper
    private async Task ValidateCourseEligibilityAsync(Guid courseId, Guid tenantId)
    {
        var eligibility = await _courseCatalogGateway.GetEnrollmentEligibility(courseId);
 
        if (eligibility is null)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.CourseNotFound)
                .WithData("CourseId", courseId);
        }
 
        if (eligibility.TenantId != tenantId)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.CrossTenantAccessDenied)
                .WithData("CourseId", courseId)
                .WithData("TenantId", tenantId);
        }
 
        if (eligibility.Status != "Published" || eligibility.IsHidden)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.CourseNotEligibleForEnrollment)
                .WithData("CourseId", courseId)
                .WithData("Status", eligibility.Status)
                .WithData("IsHidden", eligibility.IsHidden);
        }
    }
}