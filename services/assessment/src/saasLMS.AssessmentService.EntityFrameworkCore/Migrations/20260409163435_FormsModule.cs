using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.AssessmentService.Migrations
{
    /// <inheritdoc />
    public partial class FormsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormId",
                table: "AssessmentService_Quizzes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_Quizzes_FormId",
                table: "AssessmentService_Quizzes",
                column: "FormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssessmentService_Quizzes_FormId",
                table: "AssessmentService_Quizzes");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "AssessmentService_Quizzes");
        }
    }
}
