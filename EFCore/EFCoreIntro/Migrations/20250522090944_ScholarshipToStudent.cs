using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreIntro.Migrations
{
    /// <inheritdoc />
    public partial class ScholarshipToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Scholarship",
                table: "Students",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scholarship",
                table: "Students");
        }
    }
}
