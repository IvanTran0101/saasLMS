using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace saasLMS.CourseCatalogService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCourseCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseCatalog_Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CourseCatalog_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCatalog_Chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCatalog_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCatalog_Chapters_CourseCatalog_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "CourseCatalog_Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCatalog_Lessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ContentState = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCatalog_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCatalog_Lessons_CourseCatalog_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "CourseCatalog_Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCatalog_Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    ExternalUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextFormat = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCatalog_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCatalog_Materials_CourseCatalog_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "CourseCatalog_Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Chapters_CourseId_OrderNo",
                table: "CourseCatalog_Chapters",
                columns: new[] { "CourseId", "OrderNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Courses_InstructorId",
                table: "CourseCatalog_Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Courses_TenantId",
                table: "CourseCatalog_Courses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Courses_TenantId_Title",
                table: "CourseCatalog_Courses",
                columns: new[] { "TenantId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Lessons_ChapterId_SortOrder",
                table: "CourseCatalog_Lessons",
                columns: new[] { "ChapterId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Materials_LessonId",
                table: "CourseCatalog_Materials",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Materials_LessonId_Type",
                table: "CourseCatalog_Materials",
                columns: new[] { "LessonId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCatalog_Materials_TenantId",
                table: "CourseCatalog_Materials",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCatalog_Materials");

            migrationBuilder.DropTable(
                name: "CourseCatalog_Lessons");

            migrationBuilder.DropTable(
                name: "CourseCatalog_Chapters");

            migrationBuilder.DropTable(
                name: "CourseCatalog_Courses");
        }
    }
}
