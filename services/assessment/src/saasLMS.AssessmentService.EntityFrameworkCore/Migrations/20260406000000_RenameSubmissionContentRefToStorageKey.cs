using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using saasLMS.AssessmentService.EntityFrameworkCore;

#nullable disable

namespace saasLMS.AssessmentService.Migrations;

[DbContext(typeof(AssessmentServiceDbContext))]
[Migration("20260406000000_RenameSubmissionContentRefToStorageKey")]
public class RenameSubmissionContentRefToStorageKey : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ContentRef",
            table: "AssessmentService_Submission",
            newName: "StorageKey");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "StorageKey",
            table: "AssessmentService_Submission",
            newName: "ContentRef");
    }
}
