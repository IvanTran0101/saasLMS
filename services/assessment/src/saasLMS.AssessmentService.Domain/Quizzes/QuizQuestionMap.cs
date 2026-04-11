using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace saasLMS.AssessmentService.Quizzes;

public class QuizQuestionMap : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }
    public Guid QuizId { get; protected set; }
    public Guid FormQuestionId { get; protected set; }
    public int QuestionIndex { get; protected set; }

    protected QuizQuestionMap()
    {
    }

    public QuizQuestionMap(Guid id, Guid? tenantId, Guid quizId, Guid formQuestionId, int questionIndex)
        : base(id)
    {
        TenantId = tenantId;
        QuizId = quizId;
        FormQuestionId = formQuestionId;
        QuestionIndex = questionIndex;
    }

    public void UpdateFormQuestion(Guid formQuestionId)
    {
        FormQuestionId = formQuestionId;
    }

    public void UpdateQuestionIndex(int questionIndex)
    {
        QuestionIndex = questionIndex;
    }
}
