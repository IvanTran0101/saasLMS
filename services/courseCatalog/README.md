# Course Catalog Service — Domain Layer Overview

This repository is the **Course Catalog** microservice of the `saasLMS` system. The domain layer models the core learning‑content hierarchy and the rules around it.

## Domain Layer Structure

Location: `src/saasLMS.CourseCatalogService.Domain`

Key areas:

- `Courses/` — aggregate root and related entities
- `Settings/` — domain settings definitions

The domain layer follows ABP conventions (entities, domain services, and settings) and is persistence‑agnostic.

## Core Domain Model

### Course (Aggregate Root)

Represents a course owned by a tenant and created by an instructor.

Key fields:
- `TenantId`
- `Title` (required, unique per tenant)
- `Description`
- `Status` (e.g., Draft)
- `InstructorId`

Key behavior:
- `Rename(newTitle)` enforces non‑empty titles.

### Chapter

Represents a chapter within a course.

Key fields:
- `CourseId`
- `Title`
- `OrderNo` (must be > 0, unique per course)

### Lesson

Represents a lesson within a chapter.

Key fields:
- `ChapterId`
- `Title`
- `SortOrder` (must be > 0, unique per chapter)
- `ContentState` (e.g., Empty / HasContent)

Key behavior:
- `MarkAsContainingContent()`
- `MarkAsEmpty()`

### Material

Represents content items attached to a lesson.

Key fields:
- `TenantId`
- `LessonId`
- `Title`
- `Type` (e.g., File, ExternalUrl, Text)
- `StorageKey`, `FileName`, `MimeType`, `FileSize`
- `ExternalUrl`
- `TextContent`, `TextFormat`
- `Status` (Active / Hidden)

Key behavior:
- `SetFileContent(...)` normalizes fields for file materials
- `Hide()` / `Activate()`
- `ValidateContentByType()` is declared but not yet implemented

## Persistence Mapping (Reference)

While not part of the domain layer, EF Core mapping in
`src/saasLMS.CourseCatalogService.EntityFrameworkCore/EntityFrameworkCore/CourseCatalogServiceDbContextModelCreatingExtensions.cs`
shows how domain entities map to tables:

- `CourseCatalog_Courses`
- `CourseCatalog_Chapters`
- `CourseCatalog_Lessons`
- `CourseCatalog_Materials`

with constraints such as unique order per chapter and unique course title per tenant.

## Notes / Gaps

- `Material.ValidateContentByType()` is a stub and should enforce content rules per material type.
- Domain services and repositories are minimal; current focus is on core entities and constraints.

