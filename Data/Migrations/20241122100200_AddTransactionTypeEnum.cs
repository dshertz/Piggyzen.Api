using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsImported",
                table: "Transactions",
                newName: "TransactionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "IsImported");
        }
    }
}
