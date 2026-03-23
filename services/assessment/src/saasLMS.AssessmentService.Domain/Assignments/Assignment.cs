using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Assignments;

public class Assignment : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; }
    public string? Description { get; protected set; }
    public DateTime? Deadline { get; protected set; }
    public decimal MaxScore { get; protected set; }
    public AssignmentStatus Status { get; protected set; }
    public DateTime? PublishedAt { get; protected set; }
    public DateTime? ClosedAt { get; protected set; }
    
    protected Assignment()
    {}

    public Assignment(Guid id, Guid tenantId, Guid courseId, Guid lessonId, string title, string? description, DateTime? deadline, decimal maxScore) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("The course id cannot be empty.", nameof(courseId));
        }

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("The lesson id cannot be empty.", nameof(lessonId));
        }

        if (maxScore <= 0)
        {
            throw new ArgumentException("The max score cannot be negative.", nameof(maxScore));
        }
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        Deadline = deadline;
        MaxScore = maxScore;
        Status = AssignmentStatus.Draft;
    }

    public void UpdateInfo(string title, string? description, DateTime? deadline, decimal maxScore)
    {
        if (maxScore <= 0)
        {
            throw new ArgumentException("The max score cannot be negative.", nameof(maxScore));
        }
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        Deadline = deadline;
        MaxScore = maxScore;
    }

    public void Publish(DateTime publishedAt)
    {
        if (Status == AssignmentStatus.Published)
        {
            throw new BusinessException("Assignment is already published.");
        }

        if (Status == AssignmentStatus.Closed)
        {
            throw new BusinessException("Assignment is already closed.");
        }
        Status = AssignmentStatus.Published;
        PublishedAt = publishedAt;
        ClosedAt = null;
    }

    public void Close(DateTime closedAt)
    {
        if (Status == AssignmentStatus.Closed)
        {
            throw new BusinessException("Assignment is already closed.");
        }
        Status = AssignmentStatus.Closed;
        ClosedAt = closedAt;
    }
}