# Assessment Service API

This document lists the public HTTP API for AssessmentService. Routes are organized by aggregate and split into instructor and student use.

**Quiz**
Instructor
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| POST | `/api/quizzes` | Create quiz (draft) | `AssessmentService.Quizzes.Create` |
| PUT | `/api/quizzes/{id}` | Update quiz (draft only) | `AssessmentService.Quizzes.Update` |
| POST | `/api/quizzes/{id}/publish` | Publish quiz | `AssessmentService.Quizzes.Publish` |
| POST | `/api/quizzes/{id}/close` | Close quiz | `AssessmentService.Quizzes.Close` |
| GET | `/api/quizzes/{id}` | Get quiz (instructor view) | `AssessmentService.Quizzes.View` |
| GET | `/api/quizzes/by-course/{courseId}` | List quizzes by course | `AssessmentService.Quizzes.View` |
| GET | `/api/quizzes/by-lesson/{lessonId}` | List quizzes by lesson | `AssessmentService.Quizzes.View` |

Student
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| GET | `/api/quizzes/{id}/student` | Get quiz (Published/Closed only) | `AssessmentService.Quizzes.ViewPublished` |
| GET | `/api/quizzes/by-course/{courseId}/student` | List published quizzes by course | `AssessmentService.Quizzes.ViewPublished` |
| GET | `/api/quizzes/by-lesson/{lessonId}/student` | List published quizzes by lesson | `AssessmentService.Quizzes.ViewPublished` |

**Assignment**
Instructor
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| POST | `/api/assignments` | Create assignment (draft) | `AssessmentService.Assignments.Create` |
| PUT | `/api/assignments/{id}` | Update assignment (draft only) | `AssessmentService.Assignments.Update` |
| POST | `/api/assignments/{id}/publish` | Publish assignment | `AssessmentService.Assignments.Publish` |
| POST | `/api/assignments/{id}/close` | Close assignment | `AssessmentService.Assignments.Close` |
| GET | `/api/assignments/{id}` | Get assignment (instructor view) | `AssessmentService.Assignments.View` |
| GET | `/api/assignments/by-course/{courseId}` | List assignments by course | `AssessmentService.Assignments.View` |
| GET | `/api/assignments/by-lesson/{lessonId}` | List assignments by lesson | `AssessmentService.Assignments.View` |

Student
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| GET | `/api/assignments/{id}/student` | Get assignment (Published/Closed only) | `AssessmentService.Assignments.ViewPublished` |
| GET | `/api/assignments/by-course/{courseId}/student` | List published assignments by course | `AssessmentService.Assignments.ViewPublished` |
| GET | `/api/assignments/by-lesson/{lessonId}/student` | List published assignments by lesson | `AssessmentService.Assignments.ViewPublished` |

**Submission**
Instructor
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| POST | `/api/submissions/{id}/grade` | Grade submission | `AssessmentService.Submissions.Grade` |
| GET | `/api/submissions/{id}` | Get submission (instructor view) | `AssessmentService.Submissions.View` |
| GET | `/api/submissions/by-assignment/{assignmentId}` | List submissions by assignment | `AssessmentService.Submissions.View` |

Student
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| POST | `/api/submissions` | Submit or resubmit | `AssessmentService.Submissions.Submit` |
| GET | `/api/submissions/by-assignment/{assignmentId}/mine` | Get my submission by assignment | `AssessmentService.Submissions.ViewOwn` |

**Quiz Attempt**
Instructor
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| GET | `/api/quiz-attempts/{id}` | Get quiz attempt (instructor view) | `AssessmentService.QuizAttempts.View` |
| GET | `/api/quiz-attempts/by-quiz/{quizId}` | List attempts by quiz | `AssessmentService.QuizAttempts.View` |

Student
| Method | Path | Description | Permission |
| --- | --- | --- | --- |
| POST | `/api/quiz-attempts/start` | Start quiz attempt | `AssessmentService.QuizAttempts.Start` |
| POST | `/api/quiz-attempts/{quizId}/submit` | Submit attempt | `AssessmentService.QuizAttempts.Submit` |
| POST | `/api/quiz-attempts/{quizId}/timeout` | Timeout attempt | `AssessmentService.QuizAttempts.Submit` |
| GET | `/api/quiz-attempts/{quizId}/mine` | Get my attempt by quiz | `AssessmentService.QuizAttempts.ViewOwn` |
