using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Assignments;

namespace saasLMS.AssessmentService.Controllers;

[Route("api/assignments")]
public class AssignmentController : AssessmentServiceController
{
    private readonly IAssignmentAppService _assignmentAppService;

    public AssignmentController(IAssignmentAppService assignmentAppService)
    {
        _assignmentAppService = assignmentAppService;
    }

    [HttpPost]
    public Task<AssignmentDto> CreateAsync(CreateAssignmentDto input)
    {
        return _assignmentAppService.CreateAsync(input);
    }

    [HttpPut("{id:guid}")]
    public Task<AssignmentDto> UpdateAsync(Guid id, UpdateAssignmentDto input)
    {
        return _assignmentAppService.UpdateAsync(id, input);
    }

    [HttpPost("{id:guid}/publish")]
    public Task PublishAsync(Guid id)
    {
        return _assignmentAppService.PublishAsync(id);
    }

    [HttpPost("{id:guid}/close")]
    public Task CloseAsync(Guid id)
    {
        return _assignmentAppService.CloseAsync(id);
    }

    [HttpGet("{id:guid}")]
    public Task<AssignmentDto> GetAsync(Guid id)
    {
        return _assignmentAppService.GetAsync(id);
    }

    [HttpGet("{id:guid}/student")]
    public Task<AssignmentDto> GetStudentAsync(Guid id)
    {
        return _assignmentAppService.GetStudentAsync(id);
    }

    [HttpGet("by-course/{courseId:guid}")]
    public Task<List<AssignmentListItemDto>> GetListByCourseAsync(Guid courseId)
    {
        return _assignmentAppService.GetListByCourseAsync(courseId);
    }

    [HttpGet("by-lesson/{lessonId:guid}")]
    public Task<List<AssignmentListItemDto>> GetListByLessonAsync(Guid lessonId)
    {
        return _assignmentAppService.GetListByLessonAsync(lessonId);
    }

    [HttpGet("by-course/{courseId:guid}/student")]
    public Task<List<AssignmentListItemDto>> GetListByCourseStudentAsync(Guid courseId)
    {
        return _assignmentAppService.GetListByCourseStudentAsync(courseId);
    }

    [HttpGet("by-lesson/{lessonId:guid}/student")]
    public Task<List<AssignmentListItemDto>> GetListByLessonStudentAsync(Guid lessonId)
    {
        return _assignmentAppService.GetListByLessonStudentAsync(lessonId);
    }
}
