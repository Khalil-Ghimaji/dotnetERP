using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class ppp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AricleStocks",
                table: "AricleStocks");

            migrationBuilder.DropIndex(
                name: "IX_AricleStocks_ProduitId",
                table: "AricleStocks");

            migrationBuilder.DropColumn(
                name: "Prix",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AricleStocks");

            migrationBuilder.AddColumn<int>(
                name: "ArticleStockId",
                table: "Produits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Prix",
                table: "AricleStocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AricleStocks",
                table: "AricleStocks",
                column: "ProduitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AricleStocks",
                table: "AricleStocks");

            migrationBuilder.DropColumn(
                name: "ArticleStockId",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Prix",
                table: "AricleStocks");

            migrationBuilder.AddColumn<double>(
                name: "Prix",
                table: "Produits",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AricleStocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AricleStocks",
                table: "AricleStocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AricleStocks_ProduitId",
                table: "AricleStocks",
                column: "ProduitId",
                unique: true);
        }
    }
}
