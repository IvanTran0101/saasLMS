using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Courses;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Quizzes.Models;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using saasLMS.AssessmentService.Permissions;
using Volo.Abp.Content;
using System.IO;
using System.Text;
using System.Text.Json;
using Volo.Forms.Forms;

namespace saasLMS.AssessmentService;

public class QuizAppService : AssessmentServiceAppService, IQuizAppService
{
    private readonly IQuizRepository _quizRepository;
    private readonly QuizManager _quizManager;
    private readonly ICourseAccessChecker _courseAccessChecker;
    private readonly QuizFormManager _quizFormManager;
    private readonly IFormAppService _formAppService;

    public QuizAppService(
        IQuizRepository quizRepository,
        QuizManager quizManager,
        ICourseAccessChecker courseAccessChecker,
        QuizFormManager quizFormManager,
        IFormAppService formAppService)
    {
        _quizRepository = quizRepository;
        _quizManager = quizManager;
        _courseAccessChecker = courseAccessChecker;
        _quizFormManager = quizFormManager;
        _formAppService = formAppService;
    }

    [Authorize(AssessmentServicePermissions.Quizzes.Create)]
    public async Task<QuizDto> CreateAsync(CreateQuizDto input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var quiz = await _quizManager.CreateAsync(
            tenantId.Value,
            input.CourseId,
            input.LessonId,
            input.Title,
            input.TimeLimitMinutes,
            input.MaxScore,
            input.AttemptPolicy,
            input.QuestionsJson,
            Clock.Now);
        quiz = await _quizRepository.InsertAsync(quiz, autoSave: true);
        await _quizFormManager.SyncFormAsync(
            quiz,
            quiz.QuestionsJson,
            Clock.Now);
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.Update)]
    public async Task<QuizDto> UpdateAsync(Guid id, UpdateQuizDto input)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        Check.NotNull(input, nameof(input));
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        await _quizManager.UpdateInfoAsync(
            quiz,
            input.Title,
            input.TimeLimitMinutes,
            input.MaxScore,
            input.AttemptPolicy,
            input.QuestionsJson,
            Clock.Now);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
        await _quizFormManager.SyncFormAsync(
            quiz,
            quiz.QuestionsJson,
            Clock.Now);
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.Publish)]
    public async Task PublishAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        await _quizManager.PublishAsync(
            quiz,
            Clock.Now);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.Close)]
    public async Task CloseAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId.Value != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        await _quizManager.CloseAsync(
            quiz,
            Clock.Now);
        await _quizRepository.UpdateAsync(quiz, autoSave: true);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.View)]
    public async Task<QuizDto> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(quiz.CourseId);
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
        
    }

    [Authorize(AssessmentServicePermissions.Quizzes.ViewPublished)]
    public async Task<QuizDto> GetStudentAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(id);
        if (tenantId != quiz.TenantId)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        if (quiz.Status != QuizStatus.Published && quiz.Status != QuizStatus.Closed)
        {
            throw new BusinessException("AssessmentService:QuizNotAvailable")
                .WithData("QuizId", quiz.Id)
                .WithData("Status", quiz.Status);
        }
        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.View)]
    public async Task<List<QuizListItemDto>> GetListByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByCourseAsync(tenantId.Value, courseId);
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.View)]
    public async Task<List<QuizListItemDto>> GetListByLessonAsync(Guid lessonId)
    {
        
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByLessonAsync(tenantId.Value, lessonId);
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
        
    }

    [Authorize(AssessmentServicePermissions.Quizzes.ViewPublished)]
    public async Task<List<QuizListItemDto>> GetListByCourseStudentAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:CourseIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByCourseAsync(tenantId.Value, courseId);
        quizzes = quizzes.Where(
            x=> x.Status == QuizStatus.Published || x.Status == QuizStatus.Closed).ToList();
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.Create)]
    [RemoteService(false)]
    public async Task<QuizDto> CreateFromCsvAsync(CreateQuizFromCsvDto input, IRemoteStreamContent file)
    {
        Check.NotNull(input, nameof(input));
        if (file == null || file.ContentLength <= 0)
        {
            throw new BusinessException("AssessmentService:FileEmpty");
        }

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }

        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        var questionsJson = await ParseCsvToQuestionsJsonAsync(file);

        var attemptPolicy = AttemptPolicy.OneTime;
        var quiz = await _quizManager.CreateAsync(
            tenantId.Value,
            input.CourseId,
            input.LessonId,
            input.Title,
            input.TimeLimitMinutes,
            input.MaxScore,
            attemptPolicy,
            questionsJson,
            Clock.Now);

        var formResult = await _quizFormManager.CreateFormForNewQuizAsync(
            quiz,
            questionsJson,
            Clock.Now);

        quiz = await _quizRepository.InsertAsync(quiz, autoSave: true);
        await _quizFormManager.SyncQuestionMapsAsync(quiz, formResult, CancellationToken.None);

        return ObjectMapper.Map<Quiz, QuizDto>(quiz);
    }

    [Authorize(AssessmentServicePermissions.Quizzes.ViewPublished)]
    public async Task<QuizFormSchemaDto> GetFormSchemaAsync(Guid quizId)
    {
        if (quizId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:QuizIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quiz = await _quizRepository.GetAsync(quizId);
        if (quiz.TenantId != tenantId.Value)
        {
            throw new BusinessException("AssessmentService:QuizTenantMismatch")
                .WithData("QuizId", quiz.Id)
                .WithData("TenantId", tenantId.Value);
        }
        if (quiz.Status != QuizStatus.Published && quiz.Status != QuizStatus.Closed)
        {
            throw new BusinessException("AssessmentService:QuizNotAvailable")
                .WithData("QuizId", quiz.Id)
                .WithData("Status", quiz.Status);
        }
        quiz.EnsureFormAttached();

        var form = await _formAppService.GetAsync(quiz.FormId!.Value);
        var dto = new QuizFormSchemaDto
        {
            FormId = quiz.FormId!.Value,
            Title = form.Title ?? string.Empty,
            Description = form.Description ?? string.Empty
        };

        foreach (var q in form.Questions)
        {
            var questionDto = new QuizFormQuestionDto
            {
                Id = q.Id,
                Index = q.Index,
                Title = q.Title ?? string.Empty,
                Description = q.Description ?? string.Empty,
                IsRequired = q.IsRequired,
                QuestionType = q.QuestionType.ToString()
            };

            if (q.Choices != null)
            {
                foreach (var choice in q.Choices)
                {
                    questionDto.Choices.Add(new QuizFormChoiceDto
                    {
                        Id = choice.Id,
                        Index = choice.Index,
                        Value = choice.Value ?? string.Empty
                    });
                }
            }

            dto.Questions.Add(questionDto);
        }

        return dto;
    }

    private static async Task<string> ParseCsvToQuestionsJsonAsync(IRemoteStreamContent file)
    {
        using var stream = file.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: false);

        var rawQuestions = new List<QuizQuestionJsonModel>();
        string? line;
        var isFirstRow = true;
        var rowNumber = 0;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);
            if (fields.Count == 0)
            {
                continue;
            }

            if (isFirstRow && IsHeaderRow(fields))
            {
                isFirstRow = false;
                continue;
            }
            isFirstRow = false;
            rowNumber++;

            if (fields.Count < 4)
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "Each row must have: Question, Option1, Option2, ..., CorrectIndex");
            }

            var questionText = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(questionText))
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "Question text cannot be empty.")
                    .WithData("Row", rowNumber);
            }

            var correctIndexField = fields[^1].Trim();
            if (string.IsNullOrWhiteSpace(correctIndexField))
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "CorrectIndex is required.")
                    .WithData("Row", rowNumber);
            }
            if (!int.TryParse(correctIndexField, out var correctIndex))
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "CorrectIndex must be an integer (1-based).")
                    .WithData("Row", rowNumber);
            }

            var optionFields = fields.Skip(1).Take(fields.Count - 2).ToList();
            if (optionFields.Count < 2)
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "Each question must have at least two options.")
                    .WithData("Row", rowNumber);
            }

            if (correctIndex < 1 || correctIndex > optionFields.Count)
            {
                throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                    .WithData("Reason", "CorrectIndex is out of range.")
                    .WithData("CorrectIndex", correctIndex)
                    .WithData("OptionCount", optionFields.Count)
                    .WithData("Row", rowNumber);
            }

            var answers = new List<QuizAnswerJsonModel>();
            for (var i = 0; i < optionFields.Count; i++)
            {
                var optionText = optionFields[i].Trim();
                if (string.IsNullOrWhiteSpace(optionText))
                {
                    throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                        .WithData("Reason", "Option text cannot be empty.")
                        .WithData("Row", rowNumber)
                        .WithData("OptionIndex", i + 1);
                }
                answers.Add(new QuizAnswerJsonModel
                {
                    Text = optionText,
                    IsCorrect = (i + 1) == correctIndex
                });
            }

            rawQuestions.Add(new QuizQuestionJsonModel
            {
                Text = questionText,
                Answers = answers
            });
        }

        if (rawQuestions.Count == 0)
        {
            throw new BusinessException("AssessmentService:QuizCsvInvalidFormat")
                .WithData("Reason", "CSV contains no questions.");
        }

        var json = JsonSerializer.Serialize(rawQuestions);
        QuizQuestionsJsonValidator.ValidateAndParse(json);
        return json;
    }

    private static bool IsHeaderRow(IReadOnlyList<string> fields)
    {
        if (fields.Count < 2)
        {
            return false;
        }
        var first = fields[0].Trim().ToLowerInvariant();
        var last = fields[^1].Trim().ToLowerInvariant();
        return (first == "question" || first == "questiontext") &&
               (last == "correctindex" || last == "correct");
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }
    [Authorize(AssessmentServicePermissions.Quizzes.ViewPublished)]
    public async Task<List<QuizListItemDto>> GetListByLessonStudentAsync(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("AssessmentService:LessonIdIsEmpty");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("AssessmentService:TenantIdNotFound");
        }
        var quizzes = await  _quizRepository.GetListByLessonAsync(tenantId.Value, lessonId);
        quizzes = quizzes.Where(
            x=> x.Status == QuizStatus.Published || x.Status == QuizStatus.Closed).ToList();
        return ObjectMapper.Map<List<Quiz>, List<QuizListItemDto>>(quizzes);
    }
}
