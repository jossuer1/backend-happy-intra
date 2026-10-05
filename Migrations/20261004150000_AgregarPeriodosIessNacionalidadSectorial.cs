using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPeriodosIessNacionalidadSectorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usuarios: nacionalidad (todos los existentes quedan como ECUATORIANA) y sectorial
            migrationBuilder.AddColumn<string>(
                name: "Nacionalidad",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "ECUATORIANA");

            migrationBuilder.AddColumn<string>(
                name: "Sectorial",
                table: "Usuarios",
                type: "text",
                nullable: true);

            // Familiares: cédula del cónyuge o del hijo
            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Familiar",
                type: "text",
                nullable: true);

            // Historial de entradas y salidas del IESS
            migrationBuilder.CreateTable(
                name: "PeriodosIess",
                columns: table => new
                {
                    IdPeriodoIess = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CargoIess = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosIess", x => x.IdPeriodoIess);
                    table.ForeignKey(
                        name: "FK_PeriodosIess_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosIess_IdUsuario",
                table: "PeriodosIess",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodosIess");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Familiar");

            migrationBuilder.DropColumn(
                name: "Sectorial",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nacionalidad",
                table: "Usuarios");
        }
    }
}
