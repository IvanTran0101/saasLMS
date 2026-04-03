using System;
using saasLMS.AssessmentService.Assignments.Events;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.AssessmentService.Assignments;

public class Assignment : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; } = string.Empty;
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
        
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        SetTitle(title);
        SetDescription(description);
        SetDeadline(deadline);
        SetMaxScore(maxScore);
        Status = AssignmentStatus.Draft;
    }

    public static Assignment Create(
        Guid id,
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        string title,
        string? description,
        DateTime? deadline,
        decimal maxScore)
    {
        var assignment = new Assignment(id, tenantId, courseId, lessonId, title, description, deadline, maxScore);
        assignment.AddLocalEvent(new AssignmentCreatedDomainEvent(
            assignment.Id,
            assignment.TenantId,
            assignment.CourseId,
            assignment.LessonId,
            assignment.Title,
            assignment.Deadline,
            assignment.MaxScore));
        return assignment;
    }

    public void UpdateInfo(string title, string? description, DateTime? deadline, decimal maxScore)
    {
        if (Status != AssignmentStatus.Draft)
        {
            throw new BusinessException("Only draft assignments can be updated.");
        }
        
        SetTitle(title);
        SetDescription(description);
        SetDeadline(deadline);
        SetMaxScore(maxScore);
        
        AddLocalEvent(new AssignmentUpdatedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            Title,
            Deadline,
            MaxScore));
    }

    public void Publish(DateTime publishedAt)
    {
        EnsureReadyToPublish(publishedAt);
        Status = AssignmentStatus.Published;
        PublishedAt = publishedAt;
        ClosedAt = null;
        
        AddLocalEvent(new AssignmentPublishedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            publishedAt));
    }

    public void Close(DateTime closedAt)
    {
        if (Status != AssignmentStatus.Published)
        {
            throw new BusinessException("Only published assignments can be closed.");
        }

        if (PublishedAt.HasValue && closedAt < PublishedAt.Value)
        {
            throw new BusinessException("ClosedAt cannot be earlier than PublishedAt.");
        }
       
        Status = AssignmentStatus.Closed;
        ClosedAt = closedAt;
        
        AddLocalEvent(new AssignmentClosedDomainEvent(
            Id,
            TenantId,
            CourseId,
            LessonId,
            closedAt));
    }
    private void EnsureReadyToPublish(DateTime publishedAt)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new BusinessException("Assignment title cannot be empty.");
        }

        if (MaxScore <= 0)
        {
            throw new BusinessException("Assignment Max Score must be greater than zero.");
        }

        if (Status == AssignmentStatus.Published)
        {
            throw new BusinessException("Assignment is already published.");
        }
        if (Status == AssignmentStatus.Closed)
        {
            throw new BusinessException("Assignment is already closed.");
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new BusinessException("Assignment description cannot be empty.");
        }

        if (!Deadline.HasValue || Deadline.Value <= publishedAt)
        {
            throw new BusinessException("Assignment deadline must be later than published date.");
        } 
    }
    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessException("Assignment title cannot be empty.");
        }
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    private void SetDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))        
        {
            Description = null;
        }
        else
        {
            Description = description.Trim();
        }
    }

    private void SetDeadline(DateTime? deadline)
    {
        Deadline = deadline;
    }

    private void SetMaxScore(decimal maxScore)
    {
        if (maxScore <= 0)
        {
            throw new BusinessException("Assignment max score must be greater than zero.");
        }
        MaxScore = maxScore;
    }
}