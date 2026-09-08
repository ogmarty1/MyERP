using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Query = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceChecks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceCheckOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceCheckId = table.Column<int>(type: "int", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceCheckOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceCheckOffers_PriceChecks_PriceCheckId",
                        column: x => x.PriceCheckId,
                        principalTable: "PriceChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceCheckOffers_PriceCheckId",
                table: "PriceCheckOffers",
                column: "PriceCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChecks_ProductId",
                table: "PriceChecks",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceCheckOffers");

            migrationBuilder.DropTable(
                name: "PriceChecks");
        }
    }
}
