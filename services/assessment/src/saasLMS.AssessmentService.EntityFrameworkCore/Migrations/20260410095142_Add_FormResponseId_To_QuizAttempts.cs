using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.AssessmentService.Migrations
{
    /// <inheritdoc />
    public partial class Add_FormResponseId_To_QuizAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FormResponseId",
                table: "AssessmentService_QuizAttempts",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormResponseId",
                table: "AssessmentService_QuizAttempts");
        }
    }
}
