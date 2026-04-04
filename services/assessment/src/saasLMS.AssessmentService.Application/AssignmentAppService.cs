using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
using Volo.Abp;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using saasLMS.AssessmentService.Courses;
using saasLMS.AssessmentService.Permissions;

namespace saasLMS.AssessmentService;

public class AssignmentAppService : AssessmentServiceAppService, IAssignmentAppService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly AssignmentManager _assignmentManager;
    private readonly ICourseAccessChecker _courseAccessChecker;

    public AssignmentAppService(
        IAssignmentRepository assignmentRepository,
        AssignmentManager assignmentManager,
        ICourseAccessChecker courseAccessChecker)
    {
        _assignmentRepository = assignmentRepository;
        _assignmentManager = assignmentManager;
        _courseAccessChecker = courseAccessChecker;
    }

    [Authorize(AssessmentServicePermissions.Assignments.Create)]
    public async Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var assignment = await _assignmentManager.CreateAsync(
            tenantId.Value,
            input.CourseId,
            input.LessonId,
            input.Title,
            input.Description,
            input.Deadline,
            input.MaxScore,
            Clock.Now);
        assignment = await _assignmentRepository.InsertAsync(assignment, autoSave:true);
        return ObjectMapper.Map<Assignment, AssignmentDto>(assignment);
        
    }

    [Authorize(AssessmentServicePermissions.Assignments.Update)]
    public async Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(id);
        if (tenantId.Value != assignment.TenantId)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);
        await _assignmentManager.UpdateInfoAsync(
            assignment,
            input.Title,
            input.Description,
            input.Deadline,
            input.MaxScore,
            Clock.Now);
        assignment = await _assignmentRepository.UpdateAsync(assignment, autoSave:true);
        return ObjectMapper.Map<Assignment, AssignmentDto>(assignment);
    }

    [Authorize(AssessmentServicePermissions.Assignments.Publish)]
    public async Task PublishAsync(Guid id)
    {
        
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(id);
        

        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch");
        }
        
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);
        await _assignmentManager.PublishAsync(assignment, Clock.Now);
        await _assignmentRepository.UpdateAsync(assignment, autoSave:true);
    }

    [Authorize(AssessmentServicePermissions.Assignments.Close)]
    public async Task CloseAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(id);
        
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);
        await _assignmentManager.CloseAsync(assignment, Clock.Now);
        await _assignmentRepository.UpdateAsync(assignment, autoSave:true);
    }

    [Authorize(AssessmentServicePermissions.Assignments.View)]
    public async Task<AssignmentDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(id);
        

        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);
        return ObjectMapper.Map<Assignment, AssignmentDto>(assignment);
    }

    [Authorize(AssessmentServicePermissions.Assignments.ViewPublished)]
    public async Task<AssignmentDto> GetStudentAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(id);

        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch");
        }
        if (assignment.Status != AssignmentStatus.Published && assignment.Status != AssignmentStatus.Closed)
        {
            throw new BusinessException("AssessmentService:AssignmentNotAvailable")
                .WithData("AssignmentId", assignment.Id)
                .WithData("Status", assignment.Status);
        }
        return ObjectMapper.Map<Assignment, AssignmentDto>(assignment);
    }

    [Authorize(AssessmentServicePermissions.Assignments.View)]
    public async Task<List<AssignmentListItemDto>> GetListByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignments = await _assignmentRepository.GetListByCourseAsync(tenantId.Value,  courseId);
            return ObjectMapper.Map<List<Assignment>, List<AssignmentListItemDto>>(assignments); 
    }

    [Authorize(AssessmentServicePermissions.Assignments.View)]
    public async Task<List<AssignmentListItemDto>> GetListByLessonAsync(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignments = await _assignmentRepository.GetListByLessonAsync(tenantId.Value,  lessonId);
        return ObjectMapper.Map<List<Assignment>, List<AssignmentListItemDto>>(assignments); 
    }

    [Authorize(AssessmentServicePermissions.Assignments.ViewPublished)]
    public async Task<List<AssignmentListItemDto>> GetListByCourseStudentAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");  
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");  
        }
        var assignments = await _assignmentRepository.GetListByCourseAsync(tenantId.Value,  courseId);
        assignments = assignments
            .Where(x => x.Status == AssignmentStatus.Published || x.Status == AssignmentStatus.Closed)
            .ToList();
        return ObjectMapper.Map<List<Assignment>, List<AssignmentListItemDto>>(assignments);
    }
    [Authorize(AssessmentServicePermissions.Assignments.ViewPublished)]
    public async Task<List<AssignmentListItemDto>> GetListByLessonStudentAsync(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }

        var assignments = await _assignmentRepository.GetListByLessonAsync(tenantId.Value, lessonId);

        assignments = assignments
            .Where(x => x.Status == AssignmentStatus.Published || x.Status == AssignmentStatus.Closed)
            .ToList();

        return ObjectMapper.Map<List<Assignment>, List<AssignmentListItemDto>>(assignments);
    }
    
}
