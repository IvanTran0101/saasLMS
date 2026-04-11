using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;

namespace saasLMS.AssessmentService;

[RemoteService(Name = AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Area(AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/assessment/quiz")]
public class QuizFormController : AssessmentServiceController
{
    private readonly IQuizAppService _quizAppService;

    public QuizFormController(IQuizAppService quizAppService)
    {
        _quizAppService = quizAppService;
    }

    [HttpGet("{id}/form-schema")]
    public Task<QuizFormSchemaDto> GetFormSchemaAsync(Guid id)
    {
        return _quizAppService.GetFormSchemaAsync(id);
    }
}
