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
        decimal maxScore,
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        var assignment = Assignment.Create(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            lessonId,
            title,
            description,
            deadline,
            maxScore,
            createdAt);
        return Task.FromResult(assignment);
    }

    public Task UpdateInfoAsync(
        Assignment assignment,
        string title,
        string? description,
        DateTime? deadline,
        decimal maxScore,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(assignment, nameof(assignment));
        assignment.UpdateInfo(title, description, deadline, maxScore, updatedAt);
        return Task.CompletedTask;
    }

    public Task PublishAsync(
        Assignment assignment,
        DateTime publishedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(assignment, nameof(assignment));
        assignment.Publish(publishedAt);
        return Task.CompletedTask;
    }

    public Task CloseAsync(
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
