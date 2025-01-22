using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class FacturationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Paiements",
                newName: "PaiementId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Factures",
                newName: "FactureId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Paiements",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FactureId",
                table: "Paiements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MethodePaiement",
                table: "Paiements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Montant",
                table: "Paiements",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "CommandeId",
                table: "Factures",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateGeneration",
                table: "Factures",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "MontantTotal",
                table: "Factures",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "StatusFacture",
                table: "Factures",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Factures_CommandeId",
                table: "Factures",
                column: "CommandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Factures_Commandes_CommandeId",
                table: "Factures",
                column: "CommandeId",
                principalTable: "Commandes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Factures_Commandes_CommandeId",
                table: "Factures");

            migrationBuilder.DropIndex(
                name: "IX_Factures_CommandeId",
                table: "Factures");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Paiements");

            migrationBuilder.DropColumn(
                name: "FactureId",
                table: "Paiements");

            migrationBuilder.DropColumn(
                name: "MethodePaiement",
                table: "Paiements");

            migrationBuilder.DropColumn(
                name: "Montant",
                table: "Paiements");

            migrationBuilder.DropColumn(
                name: "CommandeId",
                table: "Factures");

            migrationBuilder.DropColumn(
                name: "DateGeneration",
                table: "Factures");

            migrationBuilder.DropColumn(
                name: "MontantTotal",
                table: "Factures");

            migrationBuilder.DropColumn(
                name: "StatusFacture",
                table: "Factures");

            migrationBuilder.RenameColumn(
                name: "PaiementId",
                table: "Paiements",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FactureId",
                table: "Factures",
                newName: "Id");
        }
    }
}
