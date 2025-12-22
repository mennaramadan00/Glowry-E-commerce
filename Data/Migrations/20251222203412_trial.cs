using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Glowry.Data.Migrations
{
    /// <inheritdoc />
    public partial class trial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategId",
                table: "Categories",
                newName: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "CategId");
        }
    }
}
