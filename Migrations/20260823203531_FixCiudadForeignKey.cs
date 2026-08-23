using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class FixCiudadForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudades_Provincias_ProvinciaIdProvincia",
                table: "Ciudades");

            migrationBuilder.DropIndex(
                name: "IX_Ciudades_ProvinciaIdProvincia",
                table: "Ciudades");

            migrationBuilder.DropColumn(
                name: "ProvinciaIdProvincia",
                table: "Ciudades");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudades_IdProvincia",
                table: "Ciudades",
                column: "IdProvincia");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudades_Provincias_IdProvincia",
                table: "Ciudades",
                column: "IdProvincia",
                principalTable: "Provincias",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ciudades_Provincias_IdProvincia",
                table: "Ciudades");

            migrationBuilder.DropIndex(
                name: "IX_Ciudades_IdProvincia",
                table: "Ciudades");

            migrationBuilder.AddColumn<long>(
                name: "ProvinciaIdProvincia",
                table: "Ciudades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Ciudades_ProvinciaIdProvincia",
                table: "Ciudades",
                column: "ProvinciaIdProvincia");

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudades_Provincias_ProvinciaIdProvincia",
                table: "Ciudades",
                column: "ProvinciaIdProvincia",
                principalTable: "Provincias",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
