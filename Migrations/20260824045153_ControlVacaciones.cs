using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class ControlVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_CargoIdCargo",
                table: "Usuarios");

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
                name: "FK_Usuarios_Roles_RolIdRol",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CargoIdCargo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CiudadIdCiudad",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EstadoCivilIdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EtniaIdEtnia",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_GeneroIdGenero",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_RolIdRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CargoIdCargo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CiudadIdCiudad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EstadoCivilIdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EtniaIdEtnia",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GeneroIdGenero",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RolIdRol",
                table: "Usuarios");

            migrationBuilder.AddColumn<bool>(
                name: "TieneVacaciones",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdCargo",
                table: "Usuarios",
                column: "IdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdCiudad",
                table: "Usuarios",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEstadoCivil",
                table: "Usuarios",
                column: "IdEstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEtnia",
                table: "Usuarios",
                column: "IdEtnia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdGenero",
                table: "Usuarios",
                column: "IdGenero");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_IdCargo",
                table: "Usuarios",
                column: "IdCargo",
                principalTable: "Cargos",
                principalColumn: "IdCargo");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Ciudades_IdCiudad",
                table: "Usuarios",
                column: "IdCiudad",
                principalTable: "Ciudades",
                principalColumn: "IdCiudad");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_EstadosCiviles_IdEstadoCivil",
                table: "Usuarios",
                column: "IdEstadoCivil",
                principalTable: "EstadosCiviles",
                principalColumn: "IdEstadoCivil");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Etnias_IdEtnia",
                table: "Usuarios",
                column: "IdEtnia",
                principalTable: "Etnias",
                principalColumn: "IdEtnia");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Generos_IdGenero",
                table: "Usuarios",
                column: "IdGenero",
                principalTable: "Generos",
                principalColumn: "IdGenero");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "Roles",
                principalColumn: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_IdCargo",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Ciudades_IdCiudad",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_EstadosCiviles_IdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Etnias_IdEtnia",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Generos_IdGenero",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_IdRol",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdCargo",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdCiudad",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdEstadoCivil",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdEtnia",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdGenero",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TieneVacaciones",
                table: "Usuarios");

            migrationBuilder.AddColumn<long>(
                name: "CargoIdCargo",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CiudadIdCiudad",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EstadoCivilIdEstadoCivil",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EtniaIdEtnia",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GeneroIdGenero",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RolIdRol",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CargoIdCargo",
                table: "Usuarios",
                column: "CargoIdCargo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CiudadIdCiudad",
                table: "Usuarios",
                column: "CiudadIdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EstadoCivilIdEstadoCivil",
                table: "Usuarios",
                column: "EstadoCivilIdEstadoCivil");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EtniaIdEtnia",
                table: "Usuarios",
                column: "EtniaIdEtnia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GeneroIdGenero",
                table: "Usuarios",
                column: "GeneroIdGenero");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolIdRol",
                table: "Usuarios",
                column: "RolIdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_CargoIdCargo",
                table: "Usuarios",
                column: "CargoIdCargo",
                principalTable: "Cargos",
                principalColumn: "IdCargo");

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
                name: "FK_Usuarios_Roles_RolIdRol",
                table: "Usuarios",
                column: "RolIdRol",
                principalTable: "Roles",
                principalColumn: "IdRol");
        }
    }
}
