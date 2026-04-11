using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace saasLMS.AssessmentService.Forms;

public interface IQuizFormResponseReader
{
    Task<IReadOnlyList<QuizFormAnswer>> GetAnswersAsync(
        Guid formResponseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<QuizFormQuestionKey>> GetQuestionKeysAsync(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken cancellationToken = default);
}
