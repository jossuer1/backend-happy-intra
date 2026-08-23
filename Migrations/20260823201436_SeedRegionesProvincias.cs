using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class SeedRegionesProvincias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Regiones_RegionIdRegion",
                table: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_Provincias_RegionIdRegion",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "RegionIdRegion",
                table: "Provincias");

            migrationBuilder.InsertData(
                table: "Regiones",
                columns: new[] { "IdRegion", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1L, true, "Costa" },
                    { 2L, true, "Sierra" },
                    { 3L, true, "Amazonía" },
                    { 4L, true, "Insular" }
                });

            migrationBuilder.InsertData(
                table: "Provincias",
                columns: new[] { "IdProvincia", "Estado", "IdRegion", "Nombre" },
                values: new object[,]
                {
                    { 1L, true, 1L, "Esmeraldas" },
                    { 2L, true, 1L, "Manabí" },
                    { 3L, true, 1L, "Santo Domingo de los Tsáchilas" },
                    { 4L, true, 1L, "Los Ríos" },
                    { 5L, true, 1L, "Guayas" },
                    { 6L, true, 1L, "Santa Elena" },
                    { 7L, true, 1L, "El Oro" },
                    { 8L, true, 2L, "Carchi" },
                    { 9L, true, 2L, "Imbabura" },
                    { 10L, true, 2L, "Pichincha" },
                    { 11L, true, 2L, "Cotopaxi" },
                    { 12L, true, 2L, "Tungurahua" },
                    { 13L, true, 2L, "Bolívar" },
                    { 14L, true, 2L, "Chimborazo" },
                    { 15L, true, 2L, "Cañar" },
                    { 16L, true, 2L, "Azuay" },
                    { 17L, true, 2L, "Loja" },
                    { 18L, true, 3L, "Sucumbíos" },
                    { 19L, true, 3L, "Napo" },
                    { 20L, true, 3L, "Orellana" },
                    { 21L, true, 3L, "Pastaza" },
                    { 22L, true, 3L, "Morona Santiago" },
                    { 23L, true, 3L, "Zamora Chinchipe" },
                    { 24L, true, 4L, "Galápagos" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_IdRegion",
                table: "Provincias",
                column: "IdRegion");

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Regiones_IdRegion",
                table: "Provincias",
                column: "IdRegion",
                principalTable: "Regiones",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provincias_Regiones_IdRegion",
                table: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_Provincias_IdRegion",
                table: "Provincias");

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "Provincias",
                keyColumn: "IdProvincia",
                keyValue: 24L);

            migrationBuilder.DeleteData(
                table: "Regiones",
                keyColumn: "IdRegion",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Regiones",
                keyColumn: "IdRegion",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Regiones",
                keyColumn: "IdRegion",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Regiones",
                keyColumn: "IdRegion",
                keyValue: 4L);

            migrationBuilder.AddColumn<long>(
                name: "RegionIdRegion",
                table: "Provincias",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_RegionIdRegion",
                table: "Provincias",
                column: "RegionIdRegion");

            migrationBuilder.AddForeignKey(
                name: "FK_Provincias_Regiones_RegionIdRegion",
                table: "Provincias",
                column: "RegionIdRegion",
                principalTable: "Regiones",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
