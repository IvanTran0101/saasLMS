using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.ReportingService.Migrations
{
    /// <inheritdoc />
    public partial class Reporting_ReadModels_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassProgressViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveEnrollmentCount = table.Column<int>(type: "int", nullable: false),
                    TotalStudents = table.Column<int>(type: "int", nullable: false),
                    CompletedCount = table.Column<int>(type: "int", nullable: false),
                    InProgressCount = table.Column<int>(type: "int", nullable: false),
                    Bucket_0_25 = table.Column<int>(type: "int", nullable: false),
                    Bucket_26_50 = table.Column<int>(type: "int", nullable: false),
                    Bucket_51_75 = table.Column<int>(type: "int", nullable: false),
                    Bucket_76_99 = table.Column<int>(type: "int", nullable: false),
                    Bucket_100 = table.Column<int>(type: "int", nullable: false),
                    LastRecalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassProgressViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseOutcomeReportViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentGradedCount = table.Column<int>(type: "int", nullable: false),
                    AssignmentScoreSum = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgAssignmentScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuizCompletedCount = table.Column<int>(type: "int", nullable: false),
                    QuizScoreSum = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvgQuizScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalScoreCount = table.Column<int>(type: "int", nullable: false),
                    FinalScoreSum = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalScoreAvg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompletionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PassRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ScoreDistributionJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseOutcomeReportViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseProgressViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActiveEnrollment = table.Column<bool>(type: "bit", nullable: false),
                    CompletedLessonsCount = table.Column<int>(type: "int", nullable: false),
                    TotalLessonsCount = table.Column<int>(type: "int", nullable: false),
                    LessonCompletionPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AssignmentGradedCount = table.Column<int>(type: "int", nullable: false),
                    TotalAssignmentsCount = table.Column<int>(type: "int", nullable: false),
                    AssignmentScoreSum = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AssignmentCompletionPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgAssignmentScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuizCompletedCount = table.Column<int>(type: "int", nullable: false),
                    TotalQuizzesCount = table.Column<int>(type: "int", nullable: false),
                    QuizScoreSum = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuizCompletionPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AvgQuizScore = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OverallProgress = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LastAccessedLessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseProgressViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantSummaryReportViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalStudents = table.Column<int>(type: "int", nullable: false),
                    ActiveStudents = table.Column<int>(type: "int", nullable: false),
                    TotalInstructors = table.Column<int>(type: "int", nullable: false),
                    TotalCourses = table.Column<int>(type: "int", nullable: false),
                    ActiveCourses = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSummaryReportViews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgressViews_CourseId",
                table: "ClassProgressViews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgressViews_TenantId",
                table: "ClassProgressViews",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgressViews_TenantId_CourseId",
                table: "ClassProgressViews",
                columns: new[] { "TenantId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseOutcomeReportViews_CourseId",
                table: "CourseOutcomeReportViews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOutcomeReportViews_TenantId",
                table: "CourseOutcomeReportViews",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOutcomeReportViews_TenantId_CourseId",
                table: "CourseOutcomeReportViews",
                columns: new[] { "TenantId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseProgressViews_CourseId",
                table: "StudentCourseProgressViews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseProgressViews_StudentId",
                table: "StudentCourseProgressViews",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseProgressViews_TenantId",
                table: "StudentCourseProgressViews",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseProgressViews_TenantId_CourseId_StudentId",
                table: "StudentCourseProgressViews",
                columns: new[] { "TenantId", "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantSummaryReportViews_TenantId",
                table: "TenantSummaryReportViews",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassProgressViews");

            migrationBuilder.DropTable(
                name: "CourseOutcomeReportViews");

            migrationBuilder.DropTable(
                name: "StudentCourseProgressViews");

            migrationBuilder.DropTable(
                name: "TenantSummaryReportViews");
        }
    }
}
