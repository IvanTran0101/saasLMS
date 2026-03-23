using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Chapter : Entity<Guid>
{
    public Guid CourseId { get; protected set; }
    public string Title { get; protected set; }
    public int OrderNo { get; protected set; }
    protected  Chapter()
    {
    }

    public Chapter(Guid id, Guid courseId, string title, int orderNo) : base(id)
    {
        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course id cannot be empty.", nameof(courseId));
        }

        if (orderNo <= 0)
        {
            throw new ArgumentException("Order no must be greater than 0.", nameof(orderNo));        }
        CourseId = courseId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        OrderNo = orderNo;
    }
}