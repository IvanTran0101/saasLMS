# TODO

TODO after finishing AppServices:
- Add course ownership check in Application layer
- Create `ICourseAccessChecker`
- Use `CourseId`-based ownership for `QuizAppService` and `AssignmentAppService`
- Do not add `InstructorId` to Quiz/Assignment for now
- QuizAttempt: added `CompletionMode` to `QuizAttemptCompletedDomainEvent` payload ("Manual"/"Timeout") to map into `QuizAttemptCompletedEto`
#
TODO - Submission download:
- Add application use case to download submission file
- Instructor can download student submission files for grading
- Student can download their own submitted file
- Validate ContentType == File
- Use ContentRef + FileName + MimeType to resolve file from storage
- Add permission/ownership check before download
