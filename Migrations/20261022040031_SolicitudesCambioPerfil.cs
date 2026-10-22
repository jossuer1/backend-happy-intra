using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class SolicitudesCambioPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PuedeActualizarHasta",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SolicitudesCambioPerfil",
                columns: table => new
                {
                    IdSolicitud = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false),
                    CambiosJson = table.Column<string>(type: "jsonb", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdRevisor = table.Column<long>(type: "bigint", nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ObservacionRrhh = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCambioPerfil", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioPerfil_Usuarios_IdRevisor",
                        column: x => x.IdRevisor,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioPerfil_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioPerfil_IdRevisor",
                table: "SolicitudesCambioPerfil",
                column: "IdRevisor");

            migrationBuilder.CreateIndex(
                name: "UX_SolicitudCambioPerfil_UnaPendientePorUsuario",
                table: "SolicitudesCambioPerfil",
                column: "IdUsuario",
                unique: true,
                filter: "\"Estado\" = 'PENDIENTE'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesCambioPerfil");

            migrationBuilder.DropColumn(
                name: "PuedeActualizarHasta",
                table: "Usuarios");
        }
    }
}
