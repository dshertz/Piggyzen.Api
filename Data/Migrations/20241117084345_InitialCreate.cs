using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piggyzen.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategorizationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorizationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorizationHistory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Balance = table.Column<decimal>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReferenceId = table.Column<string>(type: "TEXT", nullable: true),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    ParentTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsImported = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsOutlayOrReturn = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdjustedAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    CategorizationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_ParentTransactionId",
                        column: x => x.ParentTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionRelation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SourceTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetTransactionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionRelation_Transactions_SourceTransactionId",
                        column: x => x.SourceTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransactionRelation_Transactions_TargetTransactionId",
                        column: x => x.TargetTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationHistory_CategoryId",
                table: "CategorizationHistory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionRelation_SourceTransactionId",
                table: "TransactionRelation",
                column: "SourceTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionRelation_TargetTransactionId",
                table: "TransactionRelation",
                column: "TargetTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ParentTransactionId",
                table: "Transactions",
                column: "ParentTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorizationHistory");

            migrationBuilder.DropTable(
                name: "TransactionRelation");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
