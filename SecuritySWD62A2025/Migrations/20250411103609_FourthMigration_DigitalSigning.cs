using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuritySWD62A2025.Migrations
{
    /// <inheritdoc />
    public partial class FourthMigration_DigitalSigning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Articles");
        }
    }
}
