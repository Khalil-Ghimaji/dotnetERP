using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class thirdmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "estRestreint",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "nbNotes",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "nom",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "note",
                table: "Clients",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "sumNotes",
                table: "Clients",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "telephone",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "estRestreint",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "nbNotes",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "nom",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "note",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "sumNotes",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "telephone",
                table: "Clients");
        }
    }
}
