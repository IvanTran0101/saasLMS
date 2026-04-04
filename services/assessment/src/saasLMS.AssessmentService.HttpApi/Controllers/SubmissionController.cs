using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Submissions;

namespace saasLMS.AssessmentService.Controllers;

[Route("api/submissions")]
public class SubmissionController : AssessmentServiceController
{
    private readonly ISubmissionAppService _submissionAppService;

    public SubmissionController(ISubmissionAppService submissionAppService)
    {
        _submissionAppService = submissionAppService;
    }

    [HttpPost]
    public Task<SubmissionDto> SubmitAsync(SubmitSubmissionDto input)
    {
        return _submissionAppService.SubmitAsync(input);
    }

    [HttpPost("{id:guid}/grade")]
    public Task<SubmissionDto> GradeAsync(Guid id, GradeSubmissionDto input)
    {
        return _submissionAppService.GradeAsync(id, input);
    }

    [HttpGet("{id:guid}")]
    public Task<SubmissionDto> GetAsync(Guid id)
    {
        return _submissionAppService.GetAsync(id);
    }

    [HttpGet("by-assignment/{assignmentId:guid}")]
    public Task<List<SubmissionListItemDto>> GetListByAssignmentAsync(Guid assignmentId)
    {
        return _submissionAppService.GetListByAssignmentAsync(assignmentId);
    }

    [HttpGet("by-assignment/{assignmentId:guid}/mine")]
    public Task<SubmissionDto?> GetMySubmissionByAssignmentAsync(Guid assignmentId)
    {
        return _submissionAppService.GetMySubmissionByAssignmentAsync(assignmentId);
    }
}
