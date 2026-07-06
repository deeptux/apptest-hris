using Hris.Demo.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hris.Demo.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260419120000_InitialApplicantFiles")]
public partial class InitialApplicantFiles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApplicantFiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                ApplicantId = table.Column<Guid>(type: "TEXT", nullable: false),
                Category = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                ObjectKey = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                ContentType = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                StorageProvider = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                UploadedAtUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicantFiles", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ApplicantFiles_ApplicantId",
            table: "ApplicantFiles",
            column: "ApplicantId");

        migrationBuilder.CreateIndex(
            name: "IX_ApplicantFiles_ApplicantId_Category_IsActive",
            table: "ApplicantFiles",
            columns: new[] { "ApplicantId", "Category", "IsActive" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ApplicantFiles");
    }
}
