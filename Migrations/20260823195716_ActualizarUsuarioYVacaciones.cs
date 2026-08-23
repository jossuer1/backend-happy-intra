using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUsuarioYVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudad_Provincia_ProvinciaIdProvincia",
                table: "Ciudad");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincia_Region_RegionIdRegion",
                table: "Provincia");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Ciudad_CiudadIdCiudad",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_EstadoCivil_EstadoCivilIdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Etnia_EtniaIdEtnia",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Genero_GeneroIdGenero",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacacion_Usuarios_IdRegistradoPor",
                table: "Vacacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacacion_Usuarios_IdUsuario",
                table: "Vacacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacacion",
                table: "Vacacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Region",
                table: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provincia",
                table: "Provincia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genero",
                table: "Genero");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Etnia",
                table: "Etnia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoCivil",
                table: "EstadoCivil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad");

            migrationBuilder.RenameTable(
                name: "Vacacion",
                newName: "Vacaciones");

            migrationBuilder.RenameTable(
                name: "Region",
                newName: "Regiones");

            migrationBuilder.RenameTable(
                name: "Provincia",
                newName: "Provincias");

            migrationBuilder.RenameTable(
                name: "Genero",
                newName: "Generos");

            migrationBuilder.RenameTable(
                name: "Etnia",
                newName: "Etnias");

            migrationBuilder.RenameTable(
                name: "EstadoCivil",
                newName: "EstadosCiviles");

            migrationBuilder.RenameTable(
                name: "Ciudad",
                newName: "Ciudades");

            migrationBuilder.RenameIndex(
                name: "IX_Vacacion_IdUsuario",
                table: "Vacaciones",
                newName: "IX_Vacaciones_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Vacacion_IdRegistradoPor",
                table: "Vacaciones",
                newName: "IX_Vacaciones_IdRegistradoPor");

            migrationBuilder.RenameIndex(
                name: "IX_Provincia_RegionIdRegion",
                table: "Provincias",
                newName: "IX_Provincias_RegionIdRegion");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudad_ProvinciaIdProvincia",
                table: "Ciudades",
                newName: "IX_Ciudades_ProvinciaIdProvincia");

            migrationBuilder.AddColumn<string>(
                name: "CorreoPersonal",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicio",
                table: "Vacaciones",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "Vacaciones",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "TipoMovimiento",
                table: "Vacaciones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacaciones",
                table: "Vacaciones",
                column: "IdVacacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Regiones",
                table: "Regiones",
                column: "IdRegion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provincias",
                table: "Provincias",
                column: "IdProvincia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Generos",
                table: "Generos",
                column: "IdGenero");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Etnias",
                table: "Etnias",
                column: "IdEtnia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadosCiviles",
                table: "EstadosCiviles",
                column: "IdEstadoCivil");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ciudades",
                table: "Ciudades",
                column: "IdCiudad");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudades_Provincias_ProvinciaIdProvincia",
                table: "Ciudades",
                column: "ProvinciaIdProvincia",
                principalTable: "Provincias",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Regiones_RegionIdRegion",
                table: "Provincias",
                column: "RegionIdRegion",
                principalTable: "Regiones",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Ciudades_CiudadIdCiudad",
                table: "Usuarios",
                column: "CiudadIdCiudad",
                principalTable: "Ciudades",
                principalColumn: "IdCiudad");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_EstadosCiviles_EstadoCivilIdEstadoCivil",
                table: "Usuarios",
                column: "EstadoCivilIdEstadoCivil",
                principalTable: "EstadosCiviles",
                principalColumn: "IdEstadoCivil");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Etnias_EtniaIdEtnia",
                table: "Usuarios",
                column: "EtniaIdEtnia",
                principalTable: "Etnias",
                principalColumn: "IdEtnia");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Generos_GeneroIdGenero",
                table: "Usuarios",
                column: "GeneroIdGenero",
                principalTable: "Generos",
                principalColumn: "IdGenero");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacaciones_Usuarios_IdRegistradoPor",
                table: "Vacaciones",
                column: "IdRegistradoPor",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacaciones_Usuarios_IdUsuario",
                table: "Vacaciones",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudades_Provincias_ProvinciaIdProvincia",
                table: "Ciudades");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Regiones_RegionIdRegion",
                table: "Provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Ciudades_CiudadIdCiudad",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_EstadosCiviles_EstadoCivilIdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Etnias_EtniaIdEtnia",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Generos_GeneroIdGenero",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacaciones_Usuarios_IdRegistradoPor",
                table: "Vacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacaciones_Usuarios_IdUsuario",
                table: "Vacaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacaciones",
                table: "Vacaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Regiones",
                table: "Regiones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provincias",
                table: "Provincias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Generos",
                table: "Generos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Etnias",
                table: "Etnias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadosCiviles",
                table: "EstadosCiviles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ciudades",
                table: "Ciudades");

            migrationBuilder.DropColumn(
                name: "CorreoPersonal",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TipoMovimiento",
                table: "Vacaciones");

            migrationBuilder.RenameTable(
                name: "Vacaciones",
                newName: "Vacacion");

            migrationBuilder.RenameTable(
                name: "Regiones",
                newName: "Region");

            migrationBuilder.RenameTable(
                name: "Provincias",
                newName: "Provincia");

            migrationBuilder.RenameTable(
                name: "Generos",
                newName: "Genero");

            migrationBuilder.RenameTable(
                name: "Etnias",
                newName: "Etnia");

            migrationBuilder.RenameTable(
                name: "EstadosCiviles",
                newName: "EstadoCivil");

            migrationBuilder.RenameTable(
                name: "Ciudades",
                newName: "Ciudad");

            migrationBuilder.RenameIndex(
                name: "IX_Vacaciones_IdUsuario",
                table: "Vacacion",
                newName: "IX_Vacacion_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Vacaciones_IdRegistradoPor",
                table: "Vacacion",
                newName: "IX_Vacacion_IdRegistradoPor");

            migrationBuilder.RenameIndex(
                name: "IX_Provincias_RegionIdRegion",
                table: "Provincia",
                newName: "IX_Provincia_RegionIdRegion");

            migrationBuilder.RenameIndex(
                name: "IX_Ciudades_ProvinciaIdProvincia",
                table: "Ciudad",
                newName: "IX_Ciudad_ProvinciaIdProvincia");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicio",
                table: "Vacacion",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "Vacacion",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacacion",
                table: "Vacacion",
                column: "IdVacacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Region",
                table: "Region",
                column: "IdRegion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provincia",
                table: "Provincia",
                column: "IdProvincia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genero",
                table: "Genero",
                column: "IdGenero");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Etnia",
                table: "Etnia",
                column: "IdEtnia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoCivil",
                table: "EstadoCivil",
                column: "IdEstadoCivil");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad",
                column: "IdCiudad");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudad_Provincia_ProvinciaIdProvincia",
                table: "Ciudad",
                column: "ProvinciaIdProvincia",
                principalTable: "Provincia",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincia_Region_RegionIdRegion",
                table: "Provincia",
                column: "RegionIdRegion",
                principalTable: "Region",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Ciudad_CiudadIdCiudad",
                table: "Usuarios",
                column: "CiudadIdCiudad",
                principalTable: "Ciudad",
                principalColumn: "IdCiudad");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_EstadoCivil_EstadoCivilIdEstadoCivil",
                table: "Usuarios",
                column: "EstadoCivilIdEstadoCivil",
                principalTable: "EstadoCivil",
                principalColumn: "IdEstadoCivil");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Etnia_EtniaIdEtnia",
                table: "Usuarios",
                column: "EtniaIdEtnia",
                principalTable: "Etnia",
                principalColumn: "IdEtnia");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Genero_GeneroIdGenero",
                table: "Usuarios",
                column: "GeneroIdGenero",
                principalTable: "Genero",
                principalColumn: "IdGenero");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacacion_Usuarios_IdRegistradoPor",
                table: "Vacacion",
                column: "IdRegistradoPor",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacacion_Usuarios_IdUsuario",
                table: "Vacacion",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
