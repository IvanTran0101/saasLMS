using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.LearningProgressService.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningProgressProjections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnrollmentProjections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentProjections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonProjections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProjections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentProjections_CourseId",
                table: "EnrollmentProjections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentProjections_StudentId",
                table: "EnrollmentProjections",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentProjections_TenantId",
                table: "EnrollmentProjections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentProjections_TenantId_CourseId_StudentId",
                table: "EnrollmentProjections",
                columns: new[] { "TenantId", "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonProjections_CourseId",
                table: "LessonProjections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProjections_LessonId",
                table: "LessonProjections",
                column: "LessonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonProjections_TenantId",
                table: "LessonProjections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProjections_TenantId_CourseId",
                table: "LessonProjections",
                columns: new[] { "TenantId", "CourseId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrollmentProjections");

            migrationBuilder.DropTable(
                name: "LessonProjections");
        }
    }
}
