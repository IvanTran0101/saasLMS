using System;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Submissions;

public class Submission : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid AssignmentId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public ContentType ContentType { get; protected set; }
    public string ContentRef { get; protected set; }
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

        if (fileSize.HasValue && fileSize.Value < 0)
        {
            throw new ArgumentException("The file size cannot be negative.", nameof(fileSize));
        }
        
        TenantId = tenantId;
        AssignmentId = assignmentId;
        StudentId = studentId;
        ContentType = contentType;
        ContentRef = Check.NotNullOrWhiteSpace(contentRef, nameof(contentRef));
        SubmittedAt = submittedAt;
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        Status = SubmissionStatus.Submitted;
    }

    public void Grade(decimal score, DateTime gradedAt)
    {
        if (score < 0)
        {
            throw new ArgumentException("The score cannot be negative.", nameof(score));
        }
        Status = SubmissionStatus.Graded;
        Score = score;
        GradedAt = gradedAt;
    }

    public void UpdateContent(ContentType contentType,
        string contentRef,
        string? fileName,
        string? mimeType,
        long? fileSize,
        DateTime submittedAt)
    {
        if (fileSize.HasValue && fileSize.Value < 0)
        {
            throw new ArgumentException("The file size cannot be negative.", nameof(fileSize));
        }
        ContentType = contentType;
        ContentRef = Check.NotNullOrWhiteSpace(contentRef, nameof(contentRef));
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        Status = SubmissionStatus.Submitted;
        Score = null;
        GradedAt = null;
        SubmittedAt = submittedAt;
    }
}