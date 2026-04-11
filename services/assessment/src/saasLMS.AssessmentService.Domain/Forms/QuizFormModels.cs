using System;
using System.Collections.Generic;

namespace saasLMS.AssessmentService.Forms;

public sealed record QuizFormSyncResult(
    Guid FormId,
    IReadOnlyDictionary<int, Guid> QuestionIdByIndex);

public sealed record QuizFormAnswer(
    Guid QuestionId,
    Guid? SelectedChoiceId,
    string? Text);

public sealed record QuizFormQuestionKey(
    Guid QuestionId,
    Guid? CorrectChoiceId,
    string? CorrectText);

public sealed record QuizScoreResult(
    decimal Score,
    int CorrectCount,
    int TotalQuestionCount);
