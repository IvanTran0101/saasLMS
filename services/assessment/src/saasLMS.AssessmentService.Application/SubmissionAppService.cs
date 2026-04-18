using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Courses;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Permissions;
using saasLMS.AssessmentService.BlobStoring;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;

namespace saasLMS.AssessmentService;

public class SubmissionAppService : AssessmentServiceAppService, ISubmissionAppService 
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly SubmissionManager _submissionManager;
    private readonly ICourseAccessChecker _courseAccessChecker;
    private readonly IBlobContainer<SubmissionFileContainer> _blobContainer;

    public SubmissionAppService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        SubmissionManager submissionManager,
        ICourseAccessChecker courseAccessChecker,
        IBlobContainer<SubmissionFileContainer> blobContainer)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _submissionManager = submissionManager;
        _courseAccessChecker = courseAccessChecker;
        _blobContainer = blobContainer;
    }

    private const long MaxSubmissionFileSize = 20 * 1024 * 1024; // 20MB
    private static readonly Dictionary<string, HashSet<string>> AllowedSubmissionFileTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [".pptx"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "application/vnd.openxmlformats-officedocument.presentationml.presentation"
            },
            [".doc"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "application/msword"
            },
            [".zip"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "application/zip",
                "application/x-zip-compressed"
            }
        };

    private static void ValidateSubmissionUpload(string fileName, string? contentType, long? contentLength)
    {
        if (contentLength <= 0)
        {
            throw new BusinessException("Assessment:FileEmpty");
        }

        if (contentLength > MaxSubmissionFileSize)
        {
            throw new BusinessException("Assessment:FileTooLarge")
                .WithData("MaxBytes", MaxSubmissionFileSize);
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedSubmissionFileTypes.ContainsKey(extension))
        {
            throw new BusinessException("Assessment:FileTypeNotAllowed")
                .WithData("Extension", extension ?? string.Empty);
        }

        if (!string.IsNullOrWhiteSpace(contentType) &&
            !AllowedSubmissionFileTypes[extension].Contains(contentType))
        {
            throw new BusinessException("Assessment:MimeTypeNotAllowed")
                .WithData("MimeType", contentType ?? string.Empty);
        }
    }

    [Authorize(AssessmentServicePermissions.Submissions.Submit)]
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
            input.StorageKey,
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

    [Authorize(AssessmentServicePermissions.Submissions.Submit)]
    [RemoteService(false)]
    public async Task<SubmissionDto> SubmitFileAsync(Guid assignmentId, IRemoteStreamContent file)
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

        var studentId = CurrentUser.Id;
        if (!studentId.HasValue)
        {
            throw new BusinessException("AssessmentService:StudentIdNotFound");
        }

        if (file == null || file.ContentLength <= 0 || string.IsNullOrWhiteSpace(file.FileName))
        {
            throw new BusinessException("Assessment:FileEmpty");
        }

        var assignment = await _assignmentRepository.GetAsync(assignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }

        var existingSubmission = await _submissionRepository.FindByAssignmentAndStudentAsync(
            tenantId.Value,
            assignment.Id,
            studentId.Value);

        var safeFileName = Path.GetFileName(file.FileName);
        ValidateSubmissionUpload(safeFileName, file.ContentType, file.ContentLength);

        var storageKey = $"submissions/{tenantId.Value}/{assignment.Id}/{studentId.Value}/{safeFileName}";
        await _blobContainer.SaveAsync(storageKey, file.GetStream());

        Submission submission;
        try
        {
            submission = await _submissionManager.SubmitAsync(
                tenantId.Value,
                assignment,
                studentId.Value,
                Clock.Now,
                ContentType.File,
                storageKey,
                safeFileName,
                file.ContentType,
                file.ContentLength);
        }
        catch
        {
            await _blobContainer.DeleteAsync(storageKey);
            throw;
        }

        if (existingSubmission == null)
        {
            submission = await _submissionRepository.InsertAsync(submission, autoSave: true);
        }
        else
        {
            submission = await _submissionRepository.UpdateAsync(submission, autoSave: true);
        }

        if (existingSubmission?.ContentType == ContentType.File &&
            !string.IsNullOrWhiteSpace(existingSubmission.StorageKey) &&
            existingSubmission.StorageKey != storageKey)
        {
            await _blobContainer.DeleteAsync(existingSubmission.StorageKey);
        }

        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    [RemoteService(false)]
    [Authorize(AssessmentServicePermissions.Submissions.Grade)]
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
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);

        await _submissionManager.GradeAsync(
            assignment,
            submission,
            input.Score,
            Clock.Now);
        submission = await _submissionRepository.UpdateAsync(submission, autoSave: true);
        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    [Authorize(AssessmentServicePermissions.Submissions.View)]
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
        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);
        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    [Authorize(AssessmentServicePermissions.Submissions.View)]
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

    [Authorize(AssessmentServicePermissions.Submissions.ViewOwn)]
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

    [Authorize(AssessmentServicePermissions.Submissions.Submit)]
    [RemoteService(false)]
    public async Task<SubmissionDto> UploadSubmissionFileAsync(UploadSubmissionFileInput input, IRemoteStreamContent file)
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

        if (input.SubmissionId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:SubmissionIdIsEmpty");
        }

        if (file == null || file.ContentLength <= 0 || string.IsNullOrWhiteSpace(file.FileName))
        {
            throw new BusinessException("Assessment:FileEmpty");
        }

        var submission = await _submissionRepository.GetAsync(input.SubmissionId);
        if (submission.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:SubmissionTenantMismatch")
                .WithData("SubmissionId", input.SubmissionId)
                .WithData("TenantId", tenantId.Value);
        }

        if (submission.StudentId != studentId.Value)
        {
            throw new AbpAuthorizationException("You are not allowed to modify this submission.");
        }

        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }

        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new BusinessException("AssessmentService:AssignmentNotAvailable")
                .WithData("AssignmentId", assignment.Id)
                .WithData("Status", assignment.Status);
        }

        if (assignment.Deadline.HasValue && Clock.Now > assignment.Deadline.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentDeadlineExceeded")
                .WithData("AssignmentId", assignment.Id)
                .WithData("Deadline", assignment.Deadline.Value)
                .WithData("SubmittedAt", Clock.Now);
        }

        var safeFileName = Path.GetFileName(file.FileName);
        ValidateSubmissionUpload(safeFileName, file.ContentType, file.ContentLength);

        if (submission.ContentType == ContentType.File &&
            !string.IsNullOrWhiteSpace(submission.StorageKey))
        {
            await _blobContainer.DeleteAsync(submission.StorageKey);
        }

        var storageKey = $"submissions/{tenantId.Value}/{assignment.Id}/{submission.Id}/{safeFileName}";
        await _blobContainer.SaveAsync(storageKey, file.GetStream());

        submission.UpdateContent(
            ContentType.File,
            storageKey,
            safeFileName,
            file.ContentType,
            file.ContentLength,
            Clock.Now);

        submission = await _submissionRepository.UpdateAsync(submission, autoSave: true);

        return ObjectMapper.Map<Submission, SubmissionDto>(submission);
    }

    [Authorize(AssessmentServicePermissions.Submissions.View)]
    public async Task<IRemoteStreamContent> DownloadSubmissionFileAsync(DownloadSubmissionFileInput input)
    {
        Check.NotNull(input, nameof(input));

        if (input.SubmissionId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:SubmissionIdIsEmpty");
        }

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }

        var submission = await _submissionRepository.GetAsync(input.SubmissionId);
        if (submission.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:SubmissionTenantMismatch")
                .WithData("SubmissionId", input.SubmissionId)
                .WithData("TenantId", tenantId.Value);
        }

        var assignment = await _assignmentRepository.GetAsync(submission.AssignmentId);
        if (assignment.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(assignment.CourseId);

        if (submission.ContentType != ContentType.File || string.IsNullOrWhiteSpace(submission.StorageKey))
        {
            throw new BusinessException("Assessment:SubmissionFileNotFound");
        }

        var stream = await _blobContainer.GetAsync(submission.StorageKey);
        if (stream == null)
        {
            throw new BusinessException("Assessment:SubmissionFileNotFound");
        }

        return new RemoteStreamContent(
            stream,
            submission.FileName,
            submission.MimeType);
    }

    [Authorize(AssessmentServicePermissions.Submissions.ViewOwn)]
    public async Task<IRemoteStreamContent> DownloadMySubmissionFileAsync(DownloadSubmissionFileInput input)
    {
        Check.NotNull(input, nameof(input));

        if (input.SubmissionId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:SubmissionIdIsEmpty");
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

        var submission = await _submissionRepository.GetAsync(input.SubmissionId);
        if (submission.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:SubmissionTenantMismatch")
                .WithData("SubmissionId", input.SubmissionId)
                .WithData("TenantId", tenantId.Value);
        }
        if (submission.StudentId != studentId.Value)
        {
            throw new AbpAuthorizationException("You are not allowed to access this submission.");
        }

        if (submission.ContentType != ContentType.File || string.IsNullOrWhiteSpace(submission.StorageKey))
        {
            throw new BusinessException("Assessment:SubmissionFileNotFound");
        }

        var stream = await _blobContainer.GetAsync(submission.StorageKey);
        if (stream == null)
        {
            throw new BusinessException("Assessment:SubmissionFileNotFound");
        }

        return new RemoteStreamContent(
            stream,
            submission.FileName,
            submission.MimeType);
    }
}
