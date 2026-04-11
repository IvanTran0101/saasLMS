using System;
using System.Collections.Generic;

namespace saasLMS.AssessmentService.Quizzes;

public class QuizFormSchemaDto
{
    public Guid FormId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuizFormQuestionDto> Questions { get; set; } = new();
}

public class QuizFormQuestionDto
{
    public Guid Id { get; set; }
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public List<QuizFormChoiceDto> Choices { get; set; } = new();
}

public class QuizFormChoiceDto
{
    public Guid Id { get; set; }
    public int Index { get; set; }
    public string Value { get; set; } = string.Empty;
}
