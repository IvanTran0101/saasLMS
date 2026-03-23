using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.CourseCatalogService.Courses;

public class Course : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public string Title { get; protected set; }
    public string? Description { get; protected set; }
    public CourseStatus Status {get; protected set; }
    public Guid InstructorId { get; protected set; }
    
    protected  Course()
    {
    }

    public Course(Guid id, Guid tenantId, string title, string? description, Guid instructorId) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }

        if (instructorId == Guid.Empty)
        {
            throw new ArgumentException("Instructor id cannot be empty.", nameof(instructorId));
        }

        TenantId = tenantId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        InstructorId = instructorId;
        Status = CourseStatus.Draft;
    }

    public void Rename(string newTitle)
    {
        Title = Check.NotNullOrWhiteSpace(newTitle, nameof(newTitle));
    }
}