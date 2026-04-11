# Assessment Service — Forms Integration Changes

Date: 2026-04-09

This document summarizes the changes made in **Assessment Service** to integrate **ABP Forms Module** as the quiz engine while keeping existing APIs and events stable.

---

## Goals

- Replace quiz question storage with **ABP Forms**.
- Keep current **Quiz / QuizAttempt** API surface for backward compatibility.
- Preserve existing **events** and contracts.

---

## High-Level Behavior

### Before
- Quiz data and questions were stored in `Quiz` entity (`QuestionsJson`).
- Submissions were graded directly from `QuestionsJson`.

### After
- Quiz questions are stored as **Forms** (ABP Forms Module).
- `Quiz` entity now **references a Form** (`FormId`).
- `QuizAttempt` is still saved (compatibility), but **responses are stored in Forms**.
- Grading is done by comparing submitted answers with `Choice.IsCorrect` in Forms.

---

## Module Dependencies (Forms Module)

Forms module dependencies were added to each layer:

- Domain: `FormsDomainModule`
- Application: `FormsApplicationModule`
- EFCore: `FormsEntityFrameworkCoreModule`
- HttpApi: `FormsHttpApiModule`

Files updated:

- `src/saasLMS.AssessmentService.Domain/AssessmentServiceDomainModule.cs`
- `src/saasLMS.AssessmentService.Application/AssessmentServiceApplicationModule.cs`
- `src/saasLMS.AssessmentService.EntityFrameworkCore/EntityFrameworkCore/AssessmentServiceEntityFrameworkCoreModule.cs`
- `src/saasLMS.AssessmentService.HttpApi/AssessmentServiceHttpApiModule.cs`

---

## Data Model Changes

### Quiz entity
- Added `FormId` to connect quiz with Forms.

File:
- `src/saasLMS.AssessmentService.Domain/Quizzes/Quiz.cs`

### EFCore mapping
- Added `FormId` column + index.

File:
- `src/saasLMS.AssessmentService.EntityFrameworkCore/EntityFrameworkCore/AssessmentServiceDbContextModelCreatingExtensions.cs`

### DTOs
- Exposed `FormId` to clients.

Files:
- `src/saasLMS.AssessmentService.Application.Contracts/Quizzes/QuizDto.cs`
- `src/saasLMS.AssessmentService.Application.Contracts/Quizzes/QuizListItemDto.cs`

---

## Quiz Lifecycle (Forms-backed)

### Create Quiz
- Create Form via `IFormAppService.CreateAsync`.
- Set form settings:
  - `IsQuiz = true`
  - `RequiresLogin = true`
  - `HasLimitOneResponsePerUser` based on `AttemptPolicy`
  - `IsAcceptingResponses = false` (draft)
- Sync questions into Forms using `CreateQuestionDto`.
- Create `Quiz` with `FormId`.

### Update Quiz
- Update Form title via `UpdateFormDto`.
- Sync questions (update/create/delete).

### Publish Quiz
- Set `IsAcceptingResponses = true`.

### Close Quiz
- Set `IsAcceptingResponses = false`.

Implementation: `src/saasLMS.AssessmentService.Application/QuizAppService.cs`

---

## Quiz Attempts / Responses

### Submit Attempt
- Convert submitted answers to Form answers (`CreateAnswerDto`).
- Save response into Forms via `IResponseAppService.SaveAnswersAsync`.
- Grade using Forms data (`Choice.IsCorrect`).
- Persist `QuizAttempt` as before (backward compatible).

Implementation: `src/saasLMS.AssessmentService.Application/QuizAttemptAppService.cs`

---

## Scoring Logic

- Score = `correctCount / totalQuestions * MaxScore`.
- Correctness is derived from Forms `Choice.IsCorrect`.
- Since Forms does not expose `ChoiceId`, grading uses **choice index + value**.

Implemented in `QuizAttemptAppService.CalculateScoreFromForm()`.

---

## Events

**All existing events remain unchanged.**
- QuizCreated / QuizUpdated / QuizPublished / QuizClosed
- QuizAttemptStarted / QuizAttemptCompleted / QuizAttemptExpired

Only difference: **Quiz now has FormId** and grading uses Forms data.

---

## Notes / Follow-up

- A migration is required for `Quiz.FormId` column.
- If needed, `FormId` can be added to ETOs for downstream consumers.
- Frontend (Blazor WASM) must render quiz from Forms questions (custom UI).

## CSV Upload (New)
- Endpoint: `POST /api/assessment/quiz/upload-csv` (multipart/form-data).
- Fields: `CourseId`, `LessonId`, `Title`, `TimeLimitMinutes`, `MaxScore`, `AttemptPolicy`, `File`.

CSV format:
```csv
Question,Option1,Option2,Option3,CorrectIndex
What is 2+2?,3,4,5,2
Capital of France?,Berlin,Paris,Rome,2
```

Validation rules:
- Each row must have at least: `Question`, `Option1`, `Option2`, `CorrectIndex`.
- `CorrectIndex` is required and 1-based.
- Options cannot be empty.
- Each question must have exactly one correct answer.

## Form Schema Endpoint (New)
- Endpoint: `GET /api/assessment/quiz/{id}/form-schema`
- Purpose: provide WASM a sanitized schema (no correct answers) for rendering quiz UI.

---

## Summary

- Forms is now the real quiz engine.
- Existing APIs and events are preserved.
- QuizAttempt remains for compatibility, but responses are stored in Forms.
