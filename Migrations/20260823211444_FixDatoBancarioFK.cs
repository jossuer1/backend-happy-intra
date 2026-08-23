using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class FixDatoBancarioFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Bancos_BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Usuarios_UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropColumn(
                name: "BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_IdBanco",
                table: "DatoBancario",
                column: "IdBanco");

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_IdUsuario",
                table: "DatoBancario",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_DatoBancario_Bancos_IdBanco",
                table: "DatoBancario",
                column: "IdBanco",
                principalTable: "Bancos",
                principalColumn: "IdBanco",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatoBancario_Usuarios_IdUsuario",
                table: "DatoBancario",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Bancos_IdBanco",
                table: "DatoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Usuarios_IdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_IdBanco",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_IdUsuario",
                table: "DatoBancario");

            migrationBuilder.AddColumn<long>(
                name: "BancoIdBanco",
                table: "DatoBancario",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UsuarioIdUsuario",
                table: "DatoBancario",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_BancoIdBanco",
                table: "DatoBancario",
                column: "BancoIdBanco");

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_UsuarioIdUsuario",
                table: "DatoBancario",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_DatoBancario_Bancos_BancoIdBanco",
                table: "DatoBancario",
                column: "BancoIdBanco",
                principalTable: "Bancos",
                principalColumn: "IdBanco",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatoBancario_Usuarios_UsuarioIdUsuario",
                table: "DatoBancario",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
