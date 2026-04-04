using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
using Volo.Abp;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace saasLMS.AssessmentService;

public class AssignmentAppService : AssessmentServiceAppService, IAssignmentAppService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly AssignmentManager _assignmentManager;

    public AssignmentAppService(IAssignmentRepository assignmentRepository, AssignmentManager assignmentManager)
    {
        _assignmentRepository = assignmentRepository;
        _assignmentManager = assignmentManager;
    }

    public async Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (tenantId == null)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
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
        
        await _assignmentManager.PublishAsync(assignment, Clock.Now);
        await _assignmentRepository.UpdateAsync(assignment, autoSave:true);
    }

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
        await _assignmentManager.CloseAsync(assignment, Clock.Now);
        await _assignmentRepository.UpdateAsync(assignment, autoSave:true);
    }

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
        return ObjectMapper.Map<Assignment, AssignmentDto>(assignment);
    }

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
