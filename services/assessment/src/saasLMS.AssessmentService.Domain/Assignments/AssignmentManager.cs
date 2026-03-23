using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.Assignments;

public class AssignmentManager : DomainService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentManager(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public Task<Assignment> CreateAsync(
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        string title,
        string? description,
        DateTime? deadline,
        decimal maxsScore,
        CancellationToken cancellationToken = default)
    {
        var assignment = new Assignment(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            lessonId,
            title,
            description,
            deadline,
            maxsScore);
        return Task.FromResult(assignment);
    }

    public Task PublicAsync(
        Assignment assignment,
        DateTime publishedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(assignment, nameof(assignment));
        assignment.Publish(publishedAt);
        return Task.CompletedTask;
    }

    public Task ClosedAsync(
        Assignment assignment,
        DateTime closedAt,
        CancellationToken cancellationToken = default
    )
    {
        Check.NotNull(assignment, nameof(assignment));
        assignment.Close(closedAt);
        return Task.CompletedTask;
    }
}