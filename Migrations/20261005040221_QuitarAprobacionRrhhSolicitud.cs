using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class QuitarAprobacionRrhhSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_Usuarios_IdRrhhAprobador",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesVacaciones_IdRrhhAprobador",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaRespuestaRrhh",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "IdRrhhAprobador",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "ObservacionRrhh",
                table: "SolicitudesVacaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRespuestaRrhh",
                table: "SolicitudesVacaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdRrhhAprobador",
                table: "SolicitudesVacaciones",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionRrhh",
                table: "SolicitudesVacaciones",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_IdRrhhAprobador",
                table: "SolicitudesVacaciones",
                column: "IdRrhhAprobador");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_Usuarios_IdRrhhAprobador",
                table: "SolicitudesVacaciones",
                column: "IdRrhhAprobador",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
