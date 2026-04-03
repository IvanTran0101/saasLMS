using System;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using ContentType = saasLMS.AssessmentService.Shared.ContentType;
//note: A. Thiếu check enrollment active
// 
// Theo use case của bạn, student chỉ được nộp bài nếu:
// 	•	đã đăng ký khóa học
// 	•	enrollment còn hợp lệB. Thiếu check assignment có thuộc course/lesson hợp lệ theo local snapshot nếu cần
// 
// Hiện manager chỉ nhận Assignment object, nên coi như assignment đã hợp lệ trong service này rồi.
// Nếu sau này Assessment cần check thêm:
// 	•	course hidden/closed
// 	•	lesson còn hợp lệ
// 
// thì đó là logic cross-service khác, không bắt buộc trong file này ngay bây giờ.
// 
// ⸻
// 
// C. Thiếu policy rõ cho “re-submit khi đang Submitted”
// 
// Hiện bạn đã chốt:
// 	•	nếu Submitted thì update content
// 	•	nếu Graded thì reject
// 
// Điều này ổn, nhưng nên xem đây là policy đã chốt chính thức:
// 	•	1 submission record / student / assignment
// 	•	update record cũ khi chưa chấm
// 	•	không tạo version mới
// 
// Nếu đó là chủ đích, file này đúng hướng.
// 
// ⸻
// 
// D. Thiếu download/view file để giảng viên chấm
// 
// Nhưng phần này không thuộc SubmissionManager.
// 
// SubmissionManager là domain service write-side.
// Việc giảng viên tải bài nộp để chấm nên nằm ở:
// 	•	Application service
// 	•	file storage abstraction/service
// 
// Submission hiện đã có đủ dữ liệu nền:
// 	•	ContentRef
// 	•	FileName
// 	•	MimeType
// 	•	FileSize
// 
// nên manager không cần thêm logic download.
// 
// ⸻
// 
namespace saasLMS.AssessmentService.Submissions;

public class SubmissionManager : DomainService
{
    private readonly ISubmissionRepository _submissionRepository;
    public SubmissionManager(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<Submission> SubmitAsync(Guid tenantId,
        Assignment assignment,
        Guid studentId,
        DateTime submittedAt,
        ContentType contentType,
        string contentRef,
        string? fileName,
        string? mimeType,
        long? fileSize,
        CancellationToken cancellationToken = default)
    {
        
        Check.NotNull(assignment, nameof(assignment));
        if (assignment.TenantId != tenantId)
        {
            throw new BusinessException("AssessmentService:AssignmentTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("TenantId", tenantId);
        }

        if (assignment.Status != AssignmentStatus.Published)
        {
            throw new BusinessException("AssessmentService:AssignmentNotAvailable")
                .WithData("AssignmentId", assignment.Id)
                .WithData("Status", assignment.Status);
        }
        if (assignment.Deadline.HasValue && submittedAt > assignment.Deadline.Value)
        {
            throw new BusinessException("AssessmentService:AssignmentDeadlineExceeded")
                .WithData("AssignmentId", assignment.Id)
                .WithData("Deadline", assignment.Deadline.Value)
                .WithData("SubmittedAt", submittedAt);
        }

        var exists = await _submissionRepository.FindByAssignmentAndStudentAsync(
            tenantId,
            assignment.Id,
            studentId,
            cancellationToken);
        if (exists == null)
        {
            var submission = Submission.Create(
                GuidGenerator.Create(),
                tenantId,
                assignment.Id,
                studentId,
                contentType,
                contentRef,
                submittedAt,
                fileName,
                mimeType,
                fileSize);
            return submission;
        }
        else
        {
            if (exists.Status == SubmissionStatus.Submitted)
            {
                if (exists.StudentId != studentId)
                {
                    throw new BusinessException("AssessmentService:StudentIdMismatch")
                        .WithData("SubmissionId", exists.Id)
                        .WithData("StudentId", exists.StudentId);
                }
                if (exists.TenantId != tenantId)
                {
                    throw new BusinessException("AssessmentService:TenantIdMismatch")
                        .WithData("SubmissionId", exists.Id)
                        .WithData("TenantId", exists.TenantId);
                }
                if (exists.AssignmentId != assignment.Id)
                {
                    throw new BusinessException("AssessmentService:AssignmentIdMismatch")
                        .WithData("AssignmentId", assignment.Id)
                        .WithData("TenantId", exists.TenantId);
                }
                exists.UpdateContent(
                    contentType,
                    contentRef,
                    fileName,
                    mimeType,
                    fileSize,
                    submittedAt);
            }

            else if (exists.Status == SubmissionStatus.Graded)
            {
                throw new BusinessException("AssessmentService:AssignmentGraded");
            }
        }
        return exists;
    }

    public Task GradeAsync(
        Assignment assignment,
        Submission submission,
        decimal score,
        DateTime gradedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(submission, nameof(submission));
        Check.NotNull(assignment, nameof(assignment));
        if (assignment.TenantId != submission.TenantId)
        {
            throw new BusinessException("AssessmentService:AssignmentSubmissionTenantMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("SubmissionId", submission.Id)
                .WithData("TenantId", assignment.TenantId);
        }

        if (assignment.Id != submission.AssignmentId)
        {
            throw new BusinessException("AssessmentService:AssignmentMismatch")
                .WithData("AssignmentId", assignment.Id)
                .WithData("SubmissionId", submission.Id);
        }
        if (score < 0 || score > assignment.MaxScore)
        {
            throw new BusinessException("AssessmentService:InvalidScore")
                .WithData("AssignmentId", assignment.Id)
                .WithData("MaxScore", assignment.MaxScore)
                .WithData("ProvidedScore", score);
        }
        submission.Grade(score, gradedAt);
        return Task.CompletedTask;
    }
}