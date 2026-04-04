using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.EnrollmentService.Migrations
{
    /// <inheritdoc />
    public partial class InitialEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnrollmentService_Enrollment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EnrollmentService_Enrollment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentService_Enrollment_CourseId",
                table: "EnrollmentService_Enrollment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentService_Enrollment_StudentId",
                table: "EnrollmentService_Enrollment",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentService_Enrollment_TenantId",
                table: "EnrollmentService_Enrollment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentService_Enrollment_TenantId_CourseId_StudentId",
                table: "EnrollmentService_Enrollment",
                columns: new[] { "TenantId", "CourseId", "StudentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrollmentService_Enrollment");
        }
    }
}
