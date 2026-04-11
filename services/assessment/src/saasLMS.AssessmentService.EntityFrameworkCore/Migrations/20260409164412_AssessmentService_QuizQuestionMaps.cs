using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.AssessmentService.Migrations
{
    /// <inheritdoc />
    public partial class AssessmentService_QuizQuestionMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssessmentService_QuizQuestionMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FormQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentService_QuizQuestionMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentService_QuizQuestionMaps_AssessmentService_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "AssessmentService_Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizQuestionMaps_QuizId",
                table: "AssessmentService_QuizQuestionMaps",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizQuestionMaps_TenantId_QuizId_FormQuestionId",
                table: "AssessmentService_QuizQuestionMaps",
                columns: new[] { "TenantId", "QuizId", "FormQuestionId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentService_QuizQuestionMaps_TenantId_QuizId_QuestionIndex",
                table: "AssessmentService_QuizQuestionMaps",
                columns: new[] { "TenantId", "QuizId", "QuestionIndex" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentService_QuizQuestionMaps");
        }
    }
}
