using System;

namespace saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;

public class CancelEnrollmentInput
{
    public Guid CourseId { get; set; }
}