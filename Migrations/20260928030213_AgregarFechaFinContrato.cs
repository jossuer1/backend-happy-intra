using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaFinContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcumulaDecimos",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CargoIess",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsJefe",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinContrato",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Jornada",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RecibeComisiones",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TipoContrato",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUnion",
                table: "Familiar",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcumulaDecimos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CargoIess",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EsJefe",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaFinContrato",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Jornada",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RecibeComisiones",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TipoContrato",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaUnion",
                table: "Familiar");
        }
    }
}
