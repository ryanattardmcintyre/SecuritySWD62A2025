using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuritySWD62A2025.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration_Digest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Digest",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Digest",
                table: "Articles");
        }
    }
}
