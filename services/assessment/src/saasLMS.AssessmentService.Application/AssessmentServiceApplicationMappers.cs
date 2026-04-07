using Riok.Mapperly.Abstractions;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.QuizAttempts;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp.Mapperly;

namespace saasLMS.AssessmentService;

// Mapperly mappers for AssessmentService.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AssignmentToAssignmentDtoMapper : MapperBase<Assignment, AssignmentDto>
{
    public override partial AssignmentDto Map(Assignment source);
    public override partial void Map(Assignment source, AssignmentDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AssignmentToAssignmentListItemDtoMapper : MapperBase<Assignment, AssignmentListItemDto>
{
    public override partial AssignmentListItemDto Map(Assignment source);
    public override partial void Map(Assignment source, AssignmentListItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class QuizToQuizDtoMapper : MapperBase<Quiz, QuizDto>
{
    public override partial QuizDto Map(Quiz source);
    public override partial void Map(Quiz source, QuizDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class QuizToQuizListItemDtoMapper : MapperBase<Quiz, QuizListItemDto>
{
    public override partial QuizListItemDto Map(Quiz source);
    public override partial void Map(Quiz source, QuizListItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SubmissionToSubmissionDtoMapper : MapperBase<Submission, SubmissionDto>
{
    public override partial SubmissionDto Map(Submission source);
    public override partial void Map(Submission source, SubmissionDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SubmissionToSubmissionListItemDtoMapper : MapperBase<Submission, SubmissionListItemDto>
{
    public override partial SubmissionListItemDto Map(Submission source);
    public override partial void Map(Submission source, SubmissionListItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class QuizAttemptToQuizAttemptDtoMapper : MapperBase<QuizAttempt, QuizAttemptDto>
{
    public override partial QuizAttemptDto Map(QuizAttempt source);
    public override partial void Map(QuizAttempt source, QuizAttemptDto destination);
}
