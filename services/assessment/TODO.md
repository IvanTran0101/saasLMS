# TODO

TODO after finishing AppServices:
- Add course ownership check in Application layer
- Create `ICourseAccessChecker`
- Use `CourseId`-based ownership for `QuizAppService` and `AssignmentAppService`
- Do not add `InstructorId` to Quiz/Assignment for now
- QuizAttempt: added `CompletionMode` to `QuizAttemptCompletedDomainEvent` payload ("Manual"/"Timeout") to map into `QuizAttemptCompletedEto`
- CourseCatalog proxy now uses ABP conventional app service routing (no `[HttpGet]/[Route]` in `ICourseCatalogAppService`)
- CheckCourseOwnership workflow (instructor):
- Create/Update/Publish/Close: call `_courseAccessChecker.CheckCanManageCourseAsync(courseId)` after tenant check and before domain action
- `GetAsync` (instructor view): call `_courseAccessChecker.CheckCanManageCourseAsync(entity.CourseId)` after tenant check
- Student views: use separate `GetStudentAsync` (Quiz/Assignment) and only allow Published/Closed
- Submission/QuizAttempt `GetAsync` (instructor view): load related Assignment/Quiz, then check course ownership via `CourseId`
#
TODO - Submission download:
- Add application use case to download submission file
- Instructor can download student submission files for grading
- Student can download their own submitted file
- Validate ContentType == File
- Use ContentRef + FileName + MimeType to resolve file from storage
- Add permission/ownership check before download
