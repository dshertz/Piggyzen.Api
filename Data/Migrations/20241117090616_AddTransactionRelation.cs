using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionRelation_Transactions_SourceTransactionId",
                table: "TransactionRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionRelation_Transactions_TargetTransactionId",
                table: "TransactionRelation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionRelation",
                table: "TransactionRelation");

            migrationBuilder.RenameTable(
                name: "TransactionRelation",
                newName: "TransactionRelations");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionRelation_TargetTransactionId",
                table: "TransactionRelations",
                newName: "IX_TransactionRelations_TargetTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionRelation_SourceTransactionId",
                table: "TransactionRelations",
                newName: "IX_TransactionRelations_SourceTransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionRelations",
                table: "TransactionRelations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionRelations_Transactions_SourceTransactionId",
                table: "TransactionRelations",
                column: "SourceTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionRelations_Transactions_TargetTransactionId",
                table: "TransactionRelations",
                column: "TargetTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionRelations_Transactions_SourceTransactionId",
                table: "TransactionRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionRelations_Transactions_TargetTransactionId",
                table: "TransactionRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionRelations",
                table: "TransactionRelations");

            migrationBuilder.RenameTable(
                name: "TransactionRelations",
                newName: "TransactionRelation");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionRelations_TargetTransactionId",
                table: "TransactionRelation",
                newName: "IX_TransactionRelation_TargetTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionRelations_SourceTransactionId",
                table: "TransactionRelation",
                newName: "IX_TransactionRelation_SourceTransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionRelation",
                table: "TransactionRelation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionRelation_Transactions_SourceTransactionId",
                table: "TransactionRelation",
                column: "SourceTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionRelation_Transactions_TargetTransactionId",
                table: "TransactionRelation",
                column: "TargetTransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
