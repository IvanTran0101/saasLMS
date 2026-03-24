using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Chapter : Entity<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public string Title { get; protected set; }
    public int OrderNo { get; protected set; }
    
    private readonly List<Lesson> _lessons;
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    protected  Chapter()
    {
        Title = string.Empty;
        _lessons = new List<Lesson>();
    }

    public Chapter(Guid id, Guid tenantId, Guid courseId, string title, int orderNo) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }
        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course id cannot be empty.", nameof(courseId));
        }

        if (orderNo <= 0)
        {
            throw new ArgumentException("Order no must be greater than 0.", nameof(orderNo));        }
        TenantId = tenantId;
        CourseId = courseId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        OrderNo = orderNo;
        _lessons = new List<Lesson>();
    }
    public void Rename(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    public void SetOrder(int orderNo)
    {
        if (orderNo <= 0)
        {
            throw new ArgumentException("Order no must be greater than 0.", nameof(orderNo));
        }

        OrderNo = orderNo;
    }

    public Lesson AddLesson(Guid lessonId, string title)
    {
        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson id cannot be empty.", nameof(lessonId));
        }
        var lesson = new Lesson(lessonId, TenantId, Id, title, _lessons.Count + 1);
        _lessons.Add(lesson);
        return lesson;
    }

    public void RemoveLesson(Guid lessonId)
    {
        var lesson = _lessons.FirstOrDefault(x => x.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        _lessons.Remove(lesson);
        NormalizeLessonsOrder();
    }

    private void NormalizeLessonsOrder()
    {
        for (int i = 0; i < _lessons.Count; i++)
        {
            _lessons[i].SetOrder(i + 1);
        }
    }
    
}