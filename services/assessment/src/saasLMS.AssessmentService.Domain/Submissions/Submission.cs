using System;
using saasLMS.AssessmentService.Shared;
using saasLMS.AssessmentService.Submissions.Events;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Submissions;

public class Submission : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid AssignmentId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public ContentType ContentType { get; protected set; }
    public string ContentRef { get; protected set; } = string.Empty;
    public DateTime SubmittedAt { get; protected set; }
    public string? FileName {get; protected set; }
    public string? MimeType { get; protected set; }
    public long? FileSize { get; protected set; }
    public decimal? Score {get; protected set; }
    public DateTime? GradedAt { get; protected set; }
    public SubmissionStatus Status { get; protected set; }
    protected Submission()
    {
    }

    public Submission(Guid id, Guid tenantId, Guid assignmentId, Guid studentId, ContentType contentType, string contentRef, DateTime submittedAt, string? fileName, string? mimeType, long? fileSize) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (assignmentId == Guid.Empty)
        {
            throw new ArgumentException("The assignment id cannot be empty.", nameof(assignmentId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("The student id cannot be empty.", nameof(studentId));
        }
        
        
        TenantId = tenantId;
        AssignmentId = assignmentId;
        StudentId = studentId;
        SetContent(contentType, contentRef, fileName, mimeType, fileSize);
        SubmittedAt = submittedAt;
        
        Status = SubmissionStatus.Submitted;
    }

    public static Submission Create(
        Guid id,
        Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        ContentType contentType,
        string contentRef,
        DateTime submittedAt,
        string? fileName,
        string? mimeType,
        long? fileSize
        )
    {
        var submission = new Submission(
			id,
			tenantId,
			assignmentId,
			studentId,
			contentType,
			contentRef,
			submittedAt,
			fileName,
			mimeType,
			fileSize);
            
        submission.AddLocalEvent(new SubmissionCreatedDomainEvent(
            submission.Id,
            submission.TenantId,
            submission.AssignmentId,
            submission.StudentId,
            submission.ContentType,
            submission.SubmittedAt));
        return submission;
    }

    public void Grade(decimal score, DateTime gradedAt)
    {
        if (Status != SubmissionStatus.Submitted)
        {
            throw new BusinessException("Only submitted submissions can be graded.");
        }
        if (score < 0)
        {
            throw new ArgumentException("The score cannot be negative.");
        }

        if (gradedAt < SubmittedAt)
        {
            throw new ArgumentException("The graded time cannot be earlier than submitted time.", nameof(gradedAt));
        }
        Status = SubmissionStatus.Graded;
        SetScore(score);
        GradedAt = gradedAt;
        
        AddLocalEvent(new SubmissionGradedDomainEvent(
            Id,
            TenantId,
            AssignmentId,
            StudentId,
            ContentType,
            score,
            gradedAt));
    }

    public void UpdateContent(ContentType contentType,
        string contentRef,
        string? fileName,
        string? mimeType,
        long? fileSize,
        DateTime submittedAt)
    {
        if (Status != SubmissionStatus.Submitted)
        {
            throw new BusinessException("Only submitted submissions can be updated.");
        }
        if (submittedAt < SubmittedAt)
        {
            throw new ArgumentException("The submitted time cannot be earlier than the current submitted time.", nameof(submittedAt));
        }
        SetContent(contentType, contentRef, fileName, mimeType, fileSize);
        SubmittedAt = submittedAt;
        
        AddLocalEvent(new SubmissionUpdatedDomainEvent(
            Id,
            TenantId,
            AssignmentId,
            StudentId,
            ContentType,
            submittedAt));
    }

    private void SetContent(
        ContentType contentType,
        string contentRef,
        string? fileName,
        string? mimeType,
        long? fileSize)
    {
        var normalizedContentRef = Check.NotNullOrWhiteSpace(contentRef, nameof(contentRef)).Trim();  
        var normalizedFileName = fileName?.Trim();
        var normalizedMimeType = mimeType?.Trim();
        switch (contentType)
        {
            case ContentType.Text:
                if (fileName != null || mimeType != null || fileSize.HasValue)
                {
                    throw new BusinessException("Text submissions cannot contain file metadata.");
                }
                FileName = null;
                MimeType = null;
                FileSize = null;
                break;
            case ContentType.File:
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new BusinessException("File submissions must contain file name.");
                }

                if (string.IsNullOrWhiteSpace(mimeType))
                {
                    throw new BusinessException("File submissions must contain mime type.");
                }

                if (!fileSize.HasValue || fileSize.Value <= 0)
                {
                    throw new BusinessException("File submissions must contain valid file size.");
                }

                break;
            case ContentType.VideoLink:
                if (!Uri.TryCreate(contentRef, UriKind.Absolute, out _))
                {
                    throw new BusinessException("Link submissions must contain a valid video URL");
                }

                FileName = null;
                MimeType = null;
                FileSize = null;

                if (fileName != null || mimeType != null || fileSize.HasValue)
                {
                    throw new BusinessException("Video submissions cannot contain file metadata.");
                }
                break;
            default:
                throw new BusinessException("Unsupported content type.");
        }
        
        
        ContentType = contentType;
        ContentRef = normalizedContentRef;
        FileName = normalizedFileName;
        MimeType = normalizedMimeType;
        FileSize = fileSize;
    }
    private void SetScore(decimal score)
    {
        if (score < 0)
        {
            throw new ArgumentException("The score cannot be negative.", nameof(score));
        }

        Score = score;
    }
}