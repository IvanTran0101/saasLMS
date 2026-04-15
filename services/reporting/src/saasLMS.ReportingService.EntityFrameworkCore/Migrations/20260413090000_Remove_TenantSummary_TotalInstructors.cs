using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.ReportingService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTenantSummaryTotalInstructors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalInstructors",
                table: "TenantSummaryReportViews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalInstructors",
                table: "TenantSummaryReportViews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
