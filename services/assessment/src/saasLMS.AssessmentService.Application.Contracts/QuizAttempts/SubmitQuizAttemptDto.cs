using System;
using System.Collections.Generic;

namespace saasLMS.AssessmentService.QuizAttempts;

public class SubmitQuizAttemptDto
{
    public Guid? FormResponseId { get; set; }
    public List<QuizAttemptAnswerDto> Answers { get; set; } = new();
    public string SubmittedAnswersJson { get; set; } = string.Empty;
}
