using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexOnCategorizationStatusAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
        name: "IX_Transactions_CategorizationStatus_Description",
        table: "Transactions",
        columns: new[] { "CategorizationStatus", "Description" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
        name: "IX_Transactions_CategorizationStatus_Description",
        table: "Transactions");
        }
    }
}
