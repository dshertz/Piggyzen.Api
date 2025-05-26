using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowSubcategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowSubcategories",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSubcategories",
                table: "Categories");
        }
    }
}
