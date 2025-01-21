using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArticleStockId",
                table: "Produits",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategorieId",
                table: "Produits",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Produits",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Prix",
                table: "Produits",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CommandeId",
                table: "ArticleCommandes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProduitId",
                table: "ArticleCommandes",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "ArticleCommandes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ProduitId",
                table: "AricleStocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "AricleStocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Produits_ArticleStockId",
                table: "Produits",
                column: "ArticleStockId");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_CategorieId",
                table: "Produits",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCommandes_CommandeId",
                table: "ArticleCommandes",
                column: "CommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCommandes_ProduitId",
                table: "ArticleCommandes",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_AricleStocks_ProduitId",
                table: "AricleStocks",
                column: "ProduitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AricleStocks_Produits_ProduitId",
                table: "AricleStocks",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCommandes_Commandes_CommandeId",
                table: "ArticleCommandes",
                column: "CommandeId",
                principalTable: "Commandes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCommandes_Produits_ProduitId",
                table: "ArticleCommandes",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_AricleStocks_ArticleStockId",
                table: "Produits",
                column: "ArticleStockId",
                principalTable: "AricleStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produits_Categories_CategorieId",
                table: "Produits",
                column: "CategorieId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AricleStocks_Produits_ProduitId",
                table: "AricleStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCommandes_Commandes_CommandeId",
                table: "ArticleCommandes");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCommandes_Produits_ProduitId",
                table: "ArticleCommandes");

            migrationBuilder.DropForeignKey(
                name: "FK_Produits_AricleStocks_ArticleStockId",
                table: "Produits");

            migrationBuilder.DropForeignKey(
                name: "FK_Produits_Categories_CategorieId",
                table: "Produits");

            migrationBuilder.DropIndex(
                name: "IX_Produits_ArticleStockId",
                table: "Produits");

            migrationBuilder.DropIndex(
                name: "IX_Produits_CategorieId",
                table: "Produits");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCommandes_CommandeId",
                table: "ArticleCommandes");

            migrationBuilder.DropIndex(
                name: "IX_ArticleCommandes_ProduitId",
                table: "ArticleCommandes");

            migrationBuilder.DropIndex(
                name: "IX_AricleStocks_ProduitId",
                table: "AricleStocks");

            migrationBuilder.DropColumn(
                name: "ArticleStockId",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "CategorieId",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Prix",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CommandeId",
                table: "ArticleCommandes");

            migrationBuilder.DropColumn(
                name: "ProduitId",
                table: "ArticleCommandes");

            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "ArticleCommandes");

            migrationBuilder.DropColumn(
                name: "ProduitId",
                table: "AricleStocks");

            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "AricleStocks");
        }
    }
}
