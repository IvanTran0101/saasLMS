using System;

namespace saasLMS.AssessmentService.Assignments;

public class CreateAssignmentDto
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public decimal MaxScore { get; set; }
    
}