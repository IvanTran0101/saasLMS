# CourseCatalog Ownership Checklist

This checklist describes what to add in CourseCatalog Service so AssessmentService can verify course ownership.

1. Define DTO
- `CourseOwnerDto` with `InstructorId: Guid`

2. Add AppService endpoint
- AppService: `CourseCatalogAppService : ApplicationService, ICourseCatalogAppService`
- Method: `GetOwnerAsync(Guid id)` returns `CourseOwnerDto`
- Conventional route (ABP):
  - `GET /api/app/course-catalog/get-owner?id={courseId}`

3. Implement ownership logic
- Load course by `id`
- Return `InstructorId` (or OwnerId) from course

4. Add permission (optional but recommended)
- Define permission (e.g. `CourseCatalog.Courses.ViewOwner`)
- Protect `GetOwnerAsync` with `[Authorize]`
- Decide if internal-only or instructor/admin only

5. Configure RemoteService
- Ensure `RemoteServices:CourseCatalog:BaseUrl` is set (already done in AssessmentService)
- Optionally set same name in CourseCatalog if needed

6. Test end-to-end
- Call endpoint directly (HTTP)
- Call via `ICourseCatalogAppService` proxy from AssessmentService
- Verify ownership check blocks non-owner instructors
