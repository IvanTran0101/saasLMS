using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;
using Volo.Abp.Content;

namespace saasLMS.AssessmentService;

[RemoteService(Name = AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Area(AssessmentServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/assessment/quiz")]
public class QuizCsvController : AssessmentServiceController
{
    private readonly IQuizAppService _quizAppService;

    public QuizCsvController(IQuizAppService quizAppService)
    {
        _quizAppService = quizAppService;
    }

    [HttpPost("upload-csv")]
    [Consumes("multipart/form-data")]
    public async Task<QuizDto> UploadQuizCsvAsync([FromForm] UploadQuizCsvRequest input)
    {
        if (input.File == null || input.File.Length <= 0)
        {
            throw new BusinessException("AssessmentService:FileEmpty");
        }

        using var stream = input.File.OpenReadStream();
        var remoteStream = new RemoteStreamContent(stream, input.File.FileName, input.File.ContentType);

        var dto = new CreateQuizFromCsvDto
        {
            CourseId = input.CourseId,
            LessonId = input.LessonId,
            Title = input.Title,
            TimeLimitMinutes = input.TimeLimitMinutes,
            MaxScore = input.MaxScore,
            AttemptPolicy = input.AttemptPolicy
        };

        return await _quizAppService.CreateFromCsvAsync(dto, remoteStream);
    }
}
