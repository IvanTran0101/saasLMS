using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using ContentType = saasLMS.AssessmentService.Shared.ContentType;

namespace saasLMS.AssessmentService.Submissions;

public class SubmissionManager : DomainService
{
    private readonly ISubmissionRepository _submissionRepository;
    public SubmissionManager(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<Submission> CreateAsync(Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        DateTime submittedAt,
        ContentType contentType,
        string contentRef,
        string? fileName,
        string? mimeType,
        long? fileSize,
        CancellationToken cancellationToken = default)
    {
        var exists = await _submissionRepository.ExistsByAssignmentAndStudentAsync(
            tenantId,
            assignmentId,
            studentId,
            cancellationToken);
        if (exists)
        {
            throw new BusinessException("AssessmentService:SubmissionAlreadyExists")
                .WithData("TenantId", tenantId)
                .WithData("AssignmentId", assignmentId)
                .WithData("StudentId", studentId);
        }

        return new Submission(
            GuidGenerator.Create(),
            tenantId,
            assignmentId,
            studentId,
            contentType,
            contentRef,
            submittedAt,
            fileName,
            mimeType,
            fileSize);
    }

    public Task GradeAsync(
        Submission submission,
        decimal score,
        DateTime gradedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(submission, nameof(submission));
        submission.Grade(score, gradedAt);
        return Task.CompletedTask;
    }
}