using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.ReportingService.Migrations
{
    /// <inheritdoc />
    public partial class Add_CourseOutcome_Totals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalAssignmentsCount",
                table: "CourseOutcomeReportViews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLessonsCount",
                table: "CourseOutcomeReportViews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuizzesCount",
                table: "CourseOutcomeReportViews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAssignmentsCount",
                table: "CourseOutcomeReportViews");

            migrationBuilder.DropColumn(
                name: "TotalLessonsCount",
                table: "CourseOutcomeReportViews");

            migrationBuilder.DropColumn(
                name: "TotalQuizzesCount",
                table: "CourseOutcomeReportViews");
        }
    }
}
