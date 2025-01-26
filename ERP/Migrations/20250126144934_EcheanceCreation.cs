using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class EcheanceCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paiements");

            migrationBuilder.AddColumn<float>(
                name: "MontantPayé",
                table: "Factures",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "Echeances",
                columns: table => new
                {
                    EcheanceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Montant = table.Column<float>(type: "REAL", nullable: false),
                    MethodePaiement = table.Column<int>(type: "INTEGER", nullable: false),
                    FactureId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatutEcheance = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Echeances", x => x.EcheanceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Echeances");

            migrationBuilder.DropColumn(
                name: "MontantPayé",
                table: "Factures");

            migrationBuilder.CreateTable(
                name: "Paiements",
                columns: table => new
                {
                    PaiementId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FactureId = table.Column<int>(type: "INTEGER", nullable: false),
                    MethodePaiement = table.Column<int>(type: "INTEGER", nullable: false),
                    Montant = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paiements", x => x.PaiementId);
                });
        }
    }
}
