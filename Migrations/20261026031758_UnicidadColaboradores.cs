using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class UnicidadColaboradores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Algunas instalaciones tenían índices no únicos creados fuera de EF.
            // Se sustituyen en la misma transacción; si hay duplicados, todo se revierte.
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Usuarios_Cedula\"");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Usuarios_CorreoEmpresa\"");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Usuarios_CorreoPersonal\"");
            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CorreoEmpresa",
                table: "Usuarios",
                column: "CorreoEmpresa",
                unique: true,
                filter: "\"CorreoEmpresa\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CorreoPersonal",
                table: "Usuarios",
                column: "CorreoPersonal",
                unique: true,
                filter: "\"CorreoPersonal\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CorreoEmpresa",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_CorreoPersonal",
                table: "Usuarios");
        }
    }
}
