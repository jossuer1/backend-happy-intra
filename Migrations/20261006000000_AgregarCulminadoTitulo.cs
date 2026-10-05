using Intranet.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    // Indica si el título está culminado o si el colaborador sigue estudiando.
    // Los títulos ya registrados quedan como culminados (defaultValue: true).
    [DbContext(typeof(AppDbContext))]
    [Migration("20261006000000_AgregarCulminadoTitulo")]
    public partial class AgregarCulminadoTitulo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Culminado",
                table: "Titulo",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Culminado",
                table: "Titulo");
        }
    }
}
