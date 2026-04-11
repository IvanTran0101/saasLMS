using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Quizzes.Models;

namespace saasLMS.AssessmentService.Forms;

public interface IQuizFormStore
{
    Task<QuizFormSyncResult> CreateFormAsync(
        Quiz quiz,
        IReadOnlyList<QuizQuestion> questions,
        DateTime now,
        CancellationToken cancellationToken = default);

    Task<QuizFormSyncResult> UpdateFormAsync(
        Quiz quiz,
        IReadOnlyList<QuizQuestion> questions,
        DateTime now,
        CancellationToken cancellationToken = default);
}
