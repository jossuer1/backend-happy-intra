using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSangreJefeDirectoYSolicitudes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdJefeDirecto",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdTipoSangre",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PuedeActualizarPerfil",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SolicitudesVacaciones",
                columns: table => new
                {
                    IdSolicitud = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiasSolicitados = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    IdJefeAprobador = table.Column<long>(type: "bigint", nullable: true),
                    FechaRespuestaJefe = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ObservacionJefe = table.Column<string>(type: "text", nullable: true),
                    IdRrhhAprobador = table.Column<long>(type: "bigint", nullable: true),
                    FechaRespuestaRrhh = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ObservacionRrhh = table.Column<string>(type: "text", nullable: true),
                    IdVacacionGenerada = table.Column<long>(type: "bigint", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesVacaciones", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Usuarios_IdJefeAprobador",
                        column: x => x.IdJefeAprobador,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Usuarios_IdRrhhAprobador",
                        column: x => x.IdRrhhAprobador,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Vacaciones_IdVacacionGenerada",
                        column: x => x.IdVacacionGenerada,
                        principalTable: "Vacaciones",
                        principalColumn: "IdVacacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposSangre",
                columns: table => new
                {
                    IdTipoSangre = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSangre", x => x.IdTipoSangre);
                });

            migrationBuilder.InsertData(
                table: "TiposSangre",
                columns: new[] { "IdTipoSangre", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1L, true, "O+" },
                    { 2L, true, "O-" },
                    { 3L, true, "A+" },
                    { 4L, true, "A-" },
                    { 5L, true, "B+" },
                    { 6L, true, "B-" },
                    { 7L, true, "AB+" },
                    { 8L, true, "AB-" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdJefeDirecto",
                table: "Usuarios",
                column: "IdJefeDirecto");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdTipoSangre",
                table: "Usuarios",
                column: "IdTipoSangre");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_IdJefeAprobador",
                table: "SolicitudesVacaciones",
                column: "IdJefeAprobador");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_IdRrhhAprobador",
                table: "SolicitudesVacaciones",
                column: "IdRrhhAprobador");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_IdUsuario",
                table: "SolicitudesVacaciones",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_IdVacacionGenerada",
                table: "SolicitudesVacaciones",
                column: "IdVacacionGenerada");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_TiposSangre_IdTipoSangre",
                table: "Usuarios",
                column: "IdTipoSangre",
                principalTable: "TiposSangre",
                principalColumn: "IdTipoSangre");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdJefeDirecto",
                table: "Usuarios",
                column: "IdJefeDirecto",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_TiposSangre_IdTipoSangre",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdJefeDirecto",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "SolicitudesVacaciones");

            migrationBuilder.DropTable(
                name: "TiposSangre");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdJefeDirecto",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdTipoSangre",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdJefeDirecto",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdTipoSangre",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PuedeActualizarPerfil",
                table: "Usuarios");
        }
    }
}
