using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp;

namespace saasLMS.AssessmentService;

public class SubmissionAppService : AssessmentServiceAppService, ISubmissionAppService 
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly SubmissionManager _submissionManager;

    public SubmissionAppService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        SubmissionManager submissionManager)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _submissionManager = submissionManager;
    }

    public async Task<SubmissionDto> SubmitAsync(SubmitSubmissionDto input)
    {
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }

        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }

        var assignment = await _assignmentRepository.GetAsync(input.AssignmentId);

        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }

        var existingSubmission = await _submissionRepository.FindByAssignmentAndStudentAsync(
            tenantId.Value,
            input.AssignmentId,
            studentId.Value);

        var submission = await _submissionManager.SubmitAsync(
            tenantId.Value,
            assignment,
            studentId.Value,
            Clock.Now,
            input.ContentType,
            input.ContentRef,
            input.FileName,
            input.MimeType,
            input.FileSize
            );

        if (existingSubmission == null)
        {
            submission = await _submissionRepository.InsertAsync(submission, autoSave: true);
        }
        else
        {
            submission = await _submissionRepository.UpdateAsync(submission, autoSave: true);
        }

        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    public async Task<SubmissionDto> GradeAsync(Guid submissionId, GradeSubmissionDto input)
    {
        Check.NotNull(input, nameof(input));
        if (submissionId == Guid.Empty)
        {
            throw new BusinessException("AssignmentService:SubmissionIdNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var submission = await _submissionRepository.GetAsync(submissionId);
        if (submission.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:SubmissionTenantMismatch")
                .WithData("SubmissionId", submissionId)
                .WithData("TenantId", tenantId.Value);
        }
        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }

        await _submissionManager.GradeAsync(
            assignment,
            submission,
            input.Score,
            Clock.Now);
        submission = await _submissionRepository.UpdateAsync(submission, autoSave: true);
        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    public async Task<SubmissionDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:SubmissionIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var submission = await _submissionRepository.GetAsync(id);
        if (submission.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:SubmissionTenantMismatch")
                .WithData("SubmissionId", id)
                .WithData("TenantId", tenantId.Value);
        }
        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    public async Task<List<SubmissionListItemDto>> GetListByAssignmentAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(assignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignmentId)
                .WithData("TenantId", tenantId.Value);
        }
        var submissions = await _submissionRepository.GetListByAssignmentAsync(tenantId.Value, assignmentId);
        return ObjectMapper.Map<List<Submission>, List<SubmissionListItemDto>>(submissions);
    }

    public async Task<SubmissionDto?> GetMySubmissionByAssignmentAsync(Guid assignmentId)
    {
        if (assignmentId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:AssignmentIdNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }
        var assignment = await _assignmentRepository.GetAsync(assignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignmentId)
                .WithData("TenantId", tenantId.Value);
        }
        var submission = await _submissionRepository.FindByAssignmentAndStudentAsync(
            tenantId.Value,
            assignmentId,
            studentId.Value);
        

        if (submission == null)
        {
            return null;
        }
        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }
}