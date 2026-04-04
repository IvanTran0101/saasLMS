using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.AssessmentService.Migrations
{
    /// <inheritdoc />
    public partial class InitialAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssessmentService_Assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentService_Assignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentService_Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "int", nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttemptPolicy = table.Column<int>(type: "int", nullable: false),
                    QuestionsJson = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentService_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentService_Submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    ContentRef = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GradedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentService_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentService_Submission_AssessmentService_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "AssessmentService_Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentService_QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentService_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentService_QuizAttempts_AssessmentService_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "AssessmentService_Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Assignment_CourseId",
                table: "AssessmentService_Assignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Assignment_LessonId",
                table: "AssessmentService_Assignment",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Assignment_TenantId",
                table: "AssessmentService_Assignment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizAttempts_QuizId",
                table: "AssessmentService_QuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizAttempts_StudentId",
                table: "AssessmentService_QuizAttempts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizAttempts_TenantId",
                table: "AssessmentService_QuizAttempts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizAttempts_TenantId_QuizId_StudentId_AttemptNumber",
                table: "AssessmentService_QuizAttempts",
                columns: new[] { "TenantId", "QuizId", "StudentId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Quizzes_CourseId",
                table: "AssessmentService_Quizzes",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Quizzes_LessonId",
                table: "AssessmentService_Quizzes",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Quizzes_TenantId",
                table: "AssessmentService_Quizzes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Submission_AssignmentId",
                table: "AssessmentService_Submission",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Submission_StudentId",
                table: "AssessmentService_Submission",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Submission_TenantId",
                table: "AssessmentService_Submission",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Submission_TenantId_AssignmentId_StudentId",
                table: "AssessmentService_Submission",
                columns: new[] { "TenantId", "AssignmentId", "StudentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentService_QuizAttempts");

            migrationBuilder.DropTable(
                name: "AssessmentService_Submission");

            migrationBuilder.DropTable(
                name: "AssessmentService_Quizzes");

            migrationBuilder.DropTable(
                name: "AssessmentService_Assignment");
        }
    }
}
