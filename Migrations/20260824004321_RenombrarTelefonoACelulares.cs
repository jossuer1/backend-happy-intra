using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarTelefonoACelulares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Usuarios",
                newName: "CelularPersonal");

            migrationBuilder.AddColumn<string>(
                name: "CelularEmpresa",
                table: "Usuarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CelularEmpresa",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "CelularPersonal",
                table: "Usuarios",
                newName: "Telefono");
        }
    }
}
