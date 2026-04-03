using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;

namespace saasLMS.AssessmentService.Submissions;

public interface ISubmissionAppService
{
    Task<SubmissionDto> SubmitAsync(SubmitSubmissionDto input);
    Task<SubmissionDto> GradeAsync(Guid submissionId, GradeSubmissionDto input);
    Task<SubmissionDto> GetAsync(Guid id);
    Task<List<SubmissionListItemDto>> GetListByAssignmentAsync(Guid assignmentId);
    Task<SubmissionDto?> GetMySubmissionByAssignmentAsync(Guid assignmentId);
}