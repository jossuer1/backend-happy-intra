using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class SeedCatalogosCompletos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_cargo_area",
                table: "cargos");

            migrationBuilder.DropForeignKey(
                name: "fk_ciudad_provincia",
                table: "ciudades");

            migrationBuilder.DropForeignKey(
                name: "FK_contactos_emergencia_usuarios_id_usuario",
                table: "contactos_emergencia");

            migrationBuilder.DropForeignKey(
                name: "FK_datos_bancarios_bancos_id_banco",
                table: "datos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "FK_datos_bancarios_usuarios_id_usuario",
                table: "datos_bancarios");

            migrationBuilder.DropForeignKey(
                name: "FK_familiares_usuarios_id_usuario",
                table: "familiares");

            migrationBuilder.DropForeignKey(
                name: "fk_provincia_region",
                table: "provincias");

            migrationBuilder.DropForeignKey(
                name: "FK_titulos_usuarios_id_usuario",
                table: "titulos");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_cargos_id_cargo",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_ciudades_id_ciudad",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_estados_civiles_id_estado_civil",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_etnias_id_etnia",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_generos_id_genero",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_roles_id_rol",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_vacaciones_usuarios_id_registrado_por",
                table: "vacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_vacaciones_usuarios_id_usuario",
                table: "vacaciones");

            migrationBuilder.DropTable(
                name: "imagenes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_cedula",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_correo_empresa",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_cargo",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_ciudad",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_estado_civil",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_etnia",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_genero",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_usuario",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_roles_nombre",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cargos",
                table: "cargos");

            migrationBuilder.DropIndex(
                name: "uq_cargo_area",
                table: "cargos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bancos",
                table: "bancos");

            migrationBuilder.DropIndex(
                name: "IX_bancos_nombre",
                table: "bancos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_areas",
                table: "areas");

            migrationBuilder.DropIndex(
                name: "IX_areas_nombre",
                table: "areas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vacaciones",
                table: "vacaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_titulos",
                table: "titulos");

            migrationBuilder.DropIndex(
                name: "IX_titulos_id_usuario",
                table: "titulos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_regiones",
                table: "regiones");

            migrationBuilder.DropIndex(
                name: "IX_regiones_nombre",
                table: "regiones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_provincias",
                table: "provincias");

            migrationBuilder.DropIndex(
                name: "uq_provincia_region",
                table: "provincias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_generos",
                table: "generos");

            migrationBuilder.DropIndex(
                name: "IX_generos_nombre",
                table: "generos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_familiares",
                table: "familiares");

            migrationBuilder.DropIndex(
                name: "IX_familiares_id_usuario",
                table: "familiares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_etnias",
                table: "etnias");

            migrationBuilder.DropIndex(
                name: "IX_etnias_nombre",
                table: "etnias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_estados_civiles",
                table: "estados_civiles");

            migrationBuilder.DropIndex(
                name: "IX_estados_civiles_nombre",
                table: "estados_civiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_datos_bancarios",
                table: "datos_bancarios");

            migrationBuilder.DropIndex(
                name: "IX_datos_bancarios_id_banco",
                table: "datos_bancarios");

            migrationBuilder.DropIndex(
                name: "IX_datos_bancarios_id_usuario",
                table: "datos_bancarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contactos_emergencia",
                table: "contactos_emergencia");

            migrationBuilder.DropIndex(
                name: "IX_contactos_emergencia_id_usuario",
                table: "contactos_emergencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ciudades",
                table: "ciudades");

            migrationBuilder.DropIndex(
                name: "uq_ciudad_provincia",
                table: "ciudades");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "cargos",
                newName: "Cargos");

            migrationBuilder.RenameTable(
                name: "bancos",
                newName: "Bancos");

            migrationBuilder.RenameTable(
                name: "areas",
                newName: "Areas");

            migrationBuilder.RenameTable(
                name: "vacaciones",
                newName: "Vacacion");

            migrationBuilder.RenameTable(
                name: "titulos",
                newName: "Titulo");

            migrationBuilder.RenameTable(
                name: "regiones",
                newName: "Region");

            migrationBuilder.RenameTable(
                name: "provincias",
                newName: "Provincia");

            migrationBuilder.RenameTable(
                name: "generos",
                newName: "Genero");

            migrationBuilder.RenameTable(
                name: "familiares",
                newName: "Familiar");

            migrationBuilder.RenameTable(
                name: "etnias",
                newName: "Etnia");

            migrationBuilder.RenameTable(
                name: "estados_civiles",
                newName: "EstadoCivil");

            migrationBuilder.RenameTable(
                name: "datos_bancarios",
                newName: "DatoBancario");

            migrationBuilder.RenameTable(
                name: "contactos_emergencia",
                newName: "ContactoEmergencia");

            migrationBuilder.RenameTable(
                name: "ciudades",
                newName: "Ciudad");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "Usuarios",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Usuarios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Usuarios",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "direccion",
                table: "Usuarios",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "cedula",
                table: "Usuarios",
                newName: "Cedula");

            migrationBuilder.RenameColumn(
                name: "apellido",
                table: "Usuarios",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "usuario",
                table: "Usuarios",
                newName: "UsuarioNombre");

            migrationBuilder.RenameColumn(
                name: "url_imagen_perfil",
                table: "Usuarios",
                newName: "UrlImagenPerfil");

            migrationBuilder.RenameColumn(
                name: "id_rol",
                table: "Usuarios",
                newName: "IdRol");

            migrationBuilder.RenameColumn(
                name: "id_genero",
                table: "Usuarios",
                newName: "IdGenero");

            migrationBuilder.RenameColumn(
                name: "id_etnia",
                table: "Usuarios",
                newName: "IdEtnia");

            migrationBuilder.RenameColumn(
                name: "id_estado_civil",
                table: "Usuarios",
                newName: "IdEstadoCivil");

            migrationBuilder.RenameColumn(
                name: "id_ciudad",
                table: "Usuarios",
                newName: "IdCiudad");

            migrationBuilder.RenameColumn(
                name: "id_cargo",
                table: "Usuarios",
                newName: "IdCargo");

            migrationBuilder.RenameColumn(
                name: "fecha_nacimiento",
                table: "Usuarios",
                newName: "FechaNacimiento");

            migrationBuilder.RenameColumn(
                name: "fecha_ingreso",
                table: "Usuarios",
                newName: "FechaIngreso");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "Usuarios",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fecha_actualizacion",
                table: "Usuarios",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "dias_vacaciones_asignados",
                table: "Usuarios",
                newName: "DiasVacacionesAsignados");

            migrationBuilder.RenameColumn(
                name: "debe_cambiar_contrasena",
                table: "Usuarios",
                newName: "DebeCambiarContrasena");

            migrationBuilder.RenameColumn(
                name: "correo_empresa",
                table: "Usuarios",
                newName: "CorreoEmpresa");

            migrationBuilder.RenameColumn(
                name: "contrasena_hash",
                table: "Usuarios",
                newName: "ContrasenaHash");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "Usuarios",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Roles",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Roles",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Roles",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "Roles",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "id_rol",
                table: "Roles",
                newName: "IdRol");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Cargos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Cargos",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Cargos",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "id_area",
                table: "Cargos",
                newName: "IdArea");

            migrationBuilder.RenameColumn(
                name: "id_cargo",
                table: "Cargos",
                newName: "IdCargo");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Bancos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Bancos",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_banco",
                table: "Bancos",
                newName: "IdBanco");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Areas",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Areas",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "descripcion",
                table: "Areas",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "id_area",
                table: "Areas",
                newName: "IdArea");

            migrationBuilder.RenameColumn(
                name: "observacion",
                table: "Vacacion",
                newName: "Observacion");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Vacacion",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "Vacacion",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "id_registrado_por",
                table: "Vacacion",
                newName: "IdRegistradoPor");

            migrationBuilder.RenameColumn(
                name: "fecha_registro",
                table: "Vacacion",
                newName: "FechaRegistro");

            migrationBuilder.RenameColumn(
                name: "fecha_inicio",
                table: "Vacacion",
                newName: "FechaInicio");

            migrationBuilder.RenameColumn(
                name: "fecha_fin",
                table: "Vacacion",
                newName: "FechaFin");

            migrationBuilder.RenameColumn(
                name: "dias_tomados",
                table: "Vacacion",
                newName: "DiasTomados");

            migrationBuilder.RenameColumn(
                name: "id_vacacion",
                table: "Vacacion",
                newName: "IdVacacion");

            migrationBuilder.RenameIndex(
                name: "IX_vacaciones_id_usuario",
                table: "Vacacion",
                newName: "IX_Vacacion_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_vacaciones_id_registrado_por",
                table: "Vacacion",
                newName: "IX_Vacacion_IdRegistradoPor");

            migrationBuilder.RenameColumn(
                name: "institucion",
                table: "Titulo",
                newName: "Institucion");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Titulo",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "nombre_titulo",
                table: "Titulo",
                newName: "NombreTitulo");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "Titulo",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "fecha_obtencion",
                table: "Titulo",
                newName: "FechaObtencion");

            migrationBuilder.RenameColumn(
                name: "id_titulo",
                table: "Titulo",
                newName: "IdTitulo");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Region",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Region",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_region",
                table: "Region",
                newName: "IdRegion");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Provincia",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Provincia",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_region",
                table: "Provincia",
                newName: "IdRegion");

            migrationBuilder.RenameColumn(
                name: "id_provincia",
                table: "Provincia",
                newName: "IdProvincia");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Genero",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Genero",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_genero",
                table: "Genero",
                newName: "IdGenero");

            migrationBuilder.RenameColumn(
                name: "parentesco",
                table: "Familiar",
                newName: "Parentesco");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Familiar",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Familiar",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "apellido",
                table: "Familiar",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "Familiar",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "fecha_nacimiento",
                table: "Familiar",
                newName: "FechaNacimiento");

            migrationBuilder.RenameColumn(
                name: "id_familiar",
                table: "Familiar",
                newName: "IdFamiliar");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Etnia",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Etnia",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_etnia",
                table: "Etnia",
                newName: "IdEtnia");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "EstadoCivil",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "EstadoCivil",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_estado_civil",
                table: "EstadoCivil",
                newName: "IdEstadoCivil");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "DatoBancario",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "tipo_cuenta",
                table: "DatoBancario",
                newName: "TipoCuenta");

            migrationBuilder.RenameColumn(
                name: "numero_cuenta",
                table: "DatoBancario",
                newName: "NumeroCuenta");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "DatoBancario",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "id_banco",
                table: "DatoBancario",
                newName: "IdBanco");

            migrationBuilder.RenameColumn(
                name: "id_dato_bancario",
                table: "DatoBancario",
                newName: "IdDatoBancario");

            migrationBuilder.RenameColumn(
                name: "telefono",
                table: "ContactoEmergencia",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "parentesco",
                table: "ContactoEmergencia",
                newName: "Parentesco");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "ContactoEmergencia",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "ContactoEmergencia",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "direccion",
                table: "ContactoEmergencia",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "apellido",
                table: "ContactoEmergencia",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "ContactoEmergencia",
                newName: "IdUsuario");

            migrationBuilder.RenameColumn(
                name: "id_contacto",
                table: "ContactoEmergencia",
                newName: "IdContacto");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Ciudad",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Ciudad",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "id_provincia",
                table: "Ciudad",
                newName: "IdProvincia");

            migrationBuilder.RenameColumn(
                name: "id_ciudad",
                table: "Ciudad",
                newName: "IdCiudad");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Usuarios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Usuarios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Apellido",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioNombre",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<int>(
                name: "DiasVacacionesAsignados",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 15);

            migrationBuilder.AlterColumn<bool>(
                name: "DebeCambiarContrasena",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "CorreoEmpresa",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ContrasenaHash",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

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
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Roles",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Roles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Cargos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Cargos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Cargos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Bancos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Bancos",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Areas",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Areas",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Areas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "Vacacion",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Vacacion",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaRegistro",
                table: "Vacacion",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "Institucion",
                table: "Titulo",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Titulo",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreTitulo",
                table: "Titulo",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<long>(
                name: "UsuarioIdUsuario",
                table: "Titulo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Region",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Region",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Provincia",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Provincia",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "RegionIdRegion",
                table: "Provincia",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Genero",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Genero",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parentesco",
                table: "Familiar",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Familiar",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Familiar",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Apellido",
                table: "Familiar",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UsuarioIdUsuario",
                table: "Familiar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Etnia",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Etnia",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "EstadoCivil",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "EstadoCivil",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "DatoBancario",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoCuenta",
                table: "DatoBancario",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroCuenta",
                table: "DatoBancario",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

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

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "ContactoEmergencia",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Parentesco",
                table: "ContactoEmergencia",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ContactoEmergencia",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "ContactoEmergencia",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "ContactoEmergencia",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Apellido",
                table: "ContactoEmergencia",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UsuarioIdUsuario",
                table: "ContactoEmergencia",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ciudad",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "Ciudad",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "ProvinciaIdProvincia",
                table: "Ciudad",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "IdRol");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos",
                column: "IdCargo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bancos",
                table: "Bancos",
                column: "IdBanco");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Areas",
                table: "Areas",
                column: "IdArea");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacacion",
                table: "Vacacion",
                column: "IdVacacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Titulo",
                table: "Titulo",
                column: "IdTitulo");

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
                name: "PK_Familiar",
                table: "Familiar",
                column: "IdFamiliar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Etnia",
                table: "Etnia",
                column: "IdEtnia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoCivil",
                table: "EstadoCivil",
                column: "IdEstadoCivil");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatoBancario",
                table: "DatoBancario",
                column: "IdDatoBancario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactoEmergencia",
                table: "ContactoEmergencia",
                column: "IdContacto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad",
                column: "IdCiudad");

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "IdArea", "Descripcion", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1L, null, true, "Gerencial" },
                    { 2L, null, true, "Tecnología" },
                    { 3L, null, true, "Crédito" },
                    { 4L, null, true, "Financiero" },
                    { 5L, null, true, "Comercial" },
                    { 6L, null, true, "Talento Humano" },
                    { 7L, null, true, "Marketing" },
                    { 8L, null, true, "Administrativo" }
                });

            migrationBuilder.InsertData(
                table: "Bancos",
                columns: new[] { "IdBanco", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1L, true, "Banco Pichincha" },
                    { 2L, true, "Banco Guayaquil" },
                    { 3L, true, "Banco del Pacífico" },
                    { 4L, true, "Produbanco" },
                    { 5L, true, "Banco Internacional" },
                    { 6L, true, "Banco del Austro" },
                    { 7L, true, "Banco Bolivariano" },
                    { 8L, true, "Banco Solidario" },
                    { 9L, true, "Banco General Rumiñahui" },
                    { 10L, true, "Banco de Machala" },
                    { 11L, true, "Banco de Loja" },
                    { 12L, true, "Banco Diners Club" },
                    { 13L, true, "Cooperativa JEP" },
                    { 14L, true, "Cooperativa Policía Nacional" },
                    { 15L, true, "Cooperativa Alianza del Valle" },
                    { 16L, true, "Cooperativa Andalucía" },
                    { 17L, true, "Cooperativa San Francisco" },
                    { 18L, true, "Mutualista Pichincha" }
                });

            migrationBuilder.InsertData(
                table: "Cargos",
                columns: new[] { "IdCargo", "Descripcion", "Estado", "IdArea", "Nombre" },
                values: new object[,]
                {
                    { 1L, null, true, 1L, "Gerente General" },
                    { 2L, null, true, 2L, "Analista de Procesos" },
                    { 3L, null, true, 3L, "Jefe de Crédito" },
                    { 4L, null, true, 2L, "Gerente de Operaciones" },
                    { 5L, null, true, 2L, "Ingeniero en Infraestructura" },
                    { 6L, null, true, 4L, "Asistente Contable" },
                    { 7L, null, true, 5L, "Ejecutivo de Cuentas" },
                    { 8L, null, true, 5L, "Promotor" },
                    { 9L, null, true, 2L, "Asistente de Infraestructura" },
                    { 10L, null, true, 6L, "Jefe de Recursos Humanos" },
                    { 11L, null, true, 5L, "Coordinador de Negocios" },
                    { 12L, null, true, 3L, "Analista de Datos" },
                    { 13L, null, true, 3L, "Monitor de Cobranza" },
                    { 14L, null, true, 7L, "Community Manager" },
                    { 15L, null, true, 3L, "Asistente de Crédito" },
                    { 16L, null, true, 8L, "Auxiliar de Servicios" },
                    { 17L, null, true, 3L, "Servicio al Cliente" },
                    { 18L, null, true, 4L, "Contador General" },
                    { 19L, null, true, 5L, "Jefe de Negocios" },
                    { 20L, null, true, 2L, "Pasante de Sistemas" },
                    { 21L, null, true, 6L, "Pasante de Talento Humano" },
                    { 22L, null, true, 7L, "Pasante de Diseño" },
                    { 23L, null, true, 4L, "Analista Contable" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_IdArea",
                table: "Cargos",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_Titulo_UsuarioIdUsuario",
                table: "Titulo",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_RegionIdRegion",
                table: "Provincia",
                column: "RegionIdRegion");

            migrationBuilder.CreateIndex(
                name: "IX_Familiar_UsuarioIdUsuario",
                table: "Familiar",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_BancoIdBanco",
                table: "DatoBancario",
                column: "BancoIdBanco");

            migrationBuilder.CreateIndex(
                name: "IX_DatoBancario_UsuarioIdUsuario",
                table: "DatoBancario",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ContactoEmergencia_UsuarioIdUsuario",
                table: "ContactoEmergencia",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Ciudad_ProvinciaIdProvincia",
                table: "Ciudad",
                column: "ProvinciaIdProvincia");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargos_Areas_IdArea",
                table: "Cargos",
                column: "IdArea",
                principalTable: "Areas",
                principalColumn: "IdArea",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ciudad_Provincia_ProvinciaIdProvincia",
                table: "Ciudad",
                column: "ProvinciaIdProvincia",
                principalTable: "Provincia",
                principalColumn: "IdProvincia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactoEmergencia_Usuarios_UsuarioIdUsuario",
                table: "ContactoEmergencia",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Familiar_Usuarios_UsuarioIdUsuario",
                table: "Familiar",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Provincia_Region_RegionIdRegion",
                table: "Provincia",
                column: "RegionIdRegion",
                principalTable: "Region",
                principalColumn: "IdRegion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Titulo_Usuarios_UsuarioIdUsuario",
                table: "Titulo",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Cargos_CargoIdCargo",
                table: "Usuarios",
                column: "CargoIdCargo",
                principalTable: "Cargos",
                principalColumn: "IdCargo");

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
                name: "FK_Usuarios_Roles_RolIdRol",
                table: "Usuarios",
                column: "RolIdRol",
                principalTable: "Roles",
                principalColumn: "IdRol",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargos_Areas_IdArea",
                table: "Cargos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ciudad_Provincia_ProvinciaIdProvincia",
                table: "Ciudad");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactoEmergencia_Usuarios_UsuarioIdUsuario",
                table: "ContactoEmergencia");

            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Bancos_BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_DatoBancario_Usuarios_UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropForeignKey(
                name: "FK_Familiar_Usuarios_UsuarioIdUsuario",
                table: "Familiar");

            migrationBuilder.DropForeignKey(
                name: "FK_Provincia_Region_RegionIdRegion",
                table: "Provincia");

            migrationBuilder.DropForeignKey(
                name: "FK_Titulo_Usuarios_UsuarioIdUsuario",
                table: "Titulo");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Cargos_CargoIdCargo",
                table: "Usuarios");

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
                name: "FK_Usuarios_Roles_RolIdRol",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacacion_Usuarios_IdRegistradoPor",
                table: "Vacacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacacion_Usuarios_IdUsuario",
                table: "Vacacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargos",
                table: "Cargos");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_IdArea",
                table: "Cargos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bancos",
                table: "Bancos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Areas",
                table: "Areas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacacion",
                table: "Vacacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Titulo",
                table: "Titulo");

            migrationBuilder.DropIndex(
                name: "IX_Titulo_UsuarioIdUsuario",
                table: "Titulo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Region",
                table: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provincia",
                table: "Provincia");

            migrationBuilder.DropIndex(
                name: "IX_Provincia_RegionIdRegion",
                table: "Provincia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genero",
                table: "Genero");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Familiar",
                table: "Familiar");

            migrationBuilder.DropIndex(
                name: "IX_Familiar_UsuarioIdUsuario",
                table: "Familiar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Etnia",
                table: "Etnia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoCivil",
                table: "EstadoCivil");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DatoBancario",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropIndex(
                name: "IX_DatoBancario_UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactoEmergencia",
                table: "ContactoEmergencia");

            migrationBuilder.DropIndex(
                name: "IX_ContactoEmergencia_UsuarioIdUsuario",
                table: "ContactoEmergencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ciudad",
                table: "Ciudad");

            migrationBuilder.DropIndex(
                name: "IX_Ciudad_ProvinciaIdProvincia",
                table: "Ciudad");

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "IdBanco",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 22L);

            migrationBuilder.DeleteData(
                table: "Cargos",
                keyColumn: "IdCargo",
                keyValue: 23L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Areas",
                keyColumn: "IdArea",
                keyValue: 8L);

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

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Titulo");

            migrationBuilder.DropColumn(
                name: "RegionIdRegion",
                table: "Provincia");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Familiar");

            migrationBuilder.DropColumn(
                name: "BancoIdBanco",
                table: "DatoBancario");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "DatoBancario");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "ContactoEmergencia");

            migrationBuilder.DropColumn(
                name: "ProvinciaIdProvincia",
                table: "Ciudad");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "Cargos",
                newName: "cargos");

            migrationBuilder.RenameTable(
                name: "Bancos",
                newName: "bancos");

            migrationBuilder.RenameTable(
                name: "Areas",
                newName: "areas");

            migrationBuilder.RenameTable(
                name: "Vacacion",
                newName: "vacaciones");

            migrationBuilder.RenameTable(
                name: "Titulo",
                newName: "titulos");

            migrationBuilder.RenameTable(
                name: "Region",
                newName: "regiones");

            migrationBuilder.RenameTable(
                name: "Provincia",
                newName: "provincias");

            migrationBuilder.RenameTable(
                name: "Genero",
                newName: "generos");

            migrationBuilder.RenameTable(
                name: "Familiar",
                newName: "familiares");

            migrationBuilder.RenameTable(
                name: "Etnia",
                newName: "etnias");

            migrationBuilder.RenameTable(
                name: "EstadoCivil",
                newName: "estados_civiles");

            migrationBuilder.RenameTable(
                name: "DatoBancario",
                newName: "datos_bancarios");

            migrationBuilder.RenameTable(
                name: "ContactoEmergencia",
                newName: "contactos_emergencia");

            migrationBuilder.RenameTable(
                name: "Ciudad",
                newName: "ciudades");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "usuarios",
                newName: "telefono");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "usuarios",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "usuarios",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "usuarios",
                newName: "direccion");

            migrationBuilder.RenameColumn(
                name: "Cedula",
                table: "usuarios",
                newName: "cedula");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "usuarios",
                newName: "apellido");

            migrationBuilder.RenameColumn(
                name: "UsuarioNombre",
                table: "usuarios",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "UrlImagenPerfil",
                table: "usuarios",
                newName: "url_imagen_perfil");

            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "usuarios",
                newName: "id_rol");

            migrationBuilder.RenameColumn(
                name: "IdGenero",
                table: "usuarios",
                newName: "id_genero");

            migrationBuilder.RenameColumn(
                name: "IdEtnia",
                table: "usuarios",
                newName: "id_etnia");

            migrationBuilder.RenameColumn(
                name: "IdEstadoCivil",
                table: "usuarios",
                newName: "id_estado_civil");

            migrationBuilder.RenameColumn(
                name: "IdCiudad",
                table: "usuarios",
                newName: "id_ciudad");

            migrationBuilder.RenameColumn(
                name: "IdCargo",
                table: "usuarios",
                newName: "id_cargo");

            migrationBuilder.RenameColumn(
                name: "FechaNacimiento",
                table: "usuarios",
                newName: "fecha_nacimiento");

            migrationBuilder.RenameColumn(
                name: "FechaIngreso",
                table: "usuarios",
                newName: "fecha_ingreso");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "usuarios",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "usuarios",
                newName: "fecha_actualizacion");

            migrationBuilder.RenameColumn(
                name: "DiasVacacionesAsignados",
                table: "usuarios",
                newName: "dias_vacaciones_asignados");

            migrationBuilder.RenameColumn(
                name: "DebeCambiarContrasena",
                table: "usuarios",
                newName: "debe_cambiar_contrasena");

            migrationBuilder.RenameColumn(
                name: "CorreoEmpresa",
                table: "usuarios",
                newName: "correo_empresa");

            migrationBuilder.RenameColumn(
                name: "ContrasenaHash",
                table: "usuarios",
                newName: "contrasena_hash");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "usuarios",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "roles",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "roles",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "roles",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "roles",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "roles",
                newName: "id_rol");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "cargos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "cargos",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "cargos",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "IdArea",
                table: "cargos",
                newName: "id_area");

            migrationBuilder.RenameColumn(
                name: "IdCargo",
                table: "cargos",
                newName: "id_cargo");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "bancos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "bancos",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdBanco",
                table: "bancos",
                newName: "id_banco");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "areas",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "areas",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "areas",
                newName: "descripcion");

            migrationBuilder.RenameColumn(
                name: "IdArea",
                table: "areas",
                newName: "id_area");

            migrationBuilder.RenameColumn(
                name: "Observacion",
                table: "vacaciones",
                newName: "observacion");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "vacaciones",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "vacaciones",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "IdRegistradoPor",
                table: "vacaciones",
                newName: "id_registrado_por");

            migrationBuilder.RenameColumn(
                name: "FechaRegistro",
                table: "vacaciones",
                newName: "fecha_registro");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "vacaciones",
                newName: "fecha_inicio");

            migrationBuilder.RenameColumn(
                name: "FechaFin",
                table: "vacaciones",
                newName: "fecha_fin");

            migrationBuilder.RenameColumn(
                name: "DiasTomados",
                table: "vacaciones",
                newName: "dias_tomados");

            migrationBuilder.RenameColumn(
                name: "IdVacacion",
                table: "vacaciones",
                newName: "id_vacacion");

            migrationBuilder.RenameIndex(
                name: "IX_Vacacion_IdUsuario",
                table: "vacaciones",
                newName: "IX_vacaciones_id_usuario");

            migrationBuilder.RenameIndex(
                name: "IX_Vacacion_IdRegistradoPor",
                table: "vacaciones",
                newName: "IX_vacaciones_id_registrado_por");

            migrationBuilder.RenameColumn(
                name: "Institucion",
                table: "titulos",
                newName: "institucion");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "titulos",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "NombreTitulo",
                table: "titulos",
                newName: "nombre_titulo");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "titulos",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "FechaObtencion",
                table: "titulos",
                newName: "fecha_obtencion");

            migrationBuilder.RenameColumn(
                name: "IdTitulo",
                table: "titulos",
                newName: "id_titulo");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "regiones",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "regiones",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdRegion",
                table: "regiones",
                newName: "id_region");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "provincias",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "provincias",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdRegion",
                table: "provincias",
                newName: "id_region");

            migrationBuilder.RenameColumn(
                name: "IdProvincia",
                table: "provincias",
                newName: "id_provincia");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "generos",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "generos",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdGenero",
                table: "generos",
                newName: "id_genero");

            migrationBuilder.RenameColumn(
                name: "Parentesco",
                table: "familiares",
                newName: "parentesco");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "familiares",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "familiares",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "familiares",
                newName: "apellido");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "familiares",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "FechaNacimiento",
                table: "familiares",
                newName: "fecha_nacimiento");

            migrationBuilder.RenameColumn(
                name: "IdFamiliar",
                table: "familiares",
                newName: "id_familiar");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "etnias",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "etnias",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdEtnia",
                table: "etnias",
                newName: "id_etnia");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "estados_civiles",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "estados_civiles",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdEstadoCivil",
                table: "estados_civiles",
                newName: "id_estado_civil");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "datos_bancarios",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "TipoCuenta",
                table: "datos_bancarios",
                newName: "tipo_cuenta");

            migrationBuilder.RenameColumn(
                name: "NumeroCuenta",
                table: "datos_bancarios",
                newName: "numero_cuenta");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "datos_bancarios",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "IdBanco",
                table: "datos_bancarios",
                newName: "id_banco");

            migrationBuilder.RenameColumn(
                name: "IdDatoBancario",
                table: "datos_bancarios",
                newName: "id_dato_bancario");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "contactos_emergencia",
                newName: "telefono");

            migrationBuilder.RenameColumn(
                name: "Parentesco",
                table: "contactos_emergencia",
                newName: "parentesco");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "contactos_emergencia",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "contactos_emergencia",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "contactos_emergencia",
                newName: "direccion");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "contactos_emergencia",
                newName: "apellido");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "contactos_emergencia",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "IdContacto",
                table: "contactos_emergencia",
                newName: "id_contacto");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "ciudades",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "ciudades",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "IdProvincia",
                table: "ciudades",
                newName: "id_provincia");

            migrationBuilder.RenameColumn(
                name: "IdCiudad",
                table: "ciudades",
                newName: "id_ciudad");

            migrationBuilder.AlterColumn<string>(
                name: "telefono",
                table: "usuarios",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "usuarios",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cedula",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "apellido",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "usuario",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_creacion",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_actualizacion",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "dias_vacaciones_asignados",
                table: "usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 15,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "debe_cambiar_contrasena",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "correo_empresa",
                table: "usuarios",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "contrasena_hash",
                table: "usuarios",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "roles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_creacion",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "cargos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "cargos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "cargos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "bancos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "bancos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "areas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "areas",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "areas",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "observacion",
                table: "vacaciones",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "vacaciones",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "vacaciones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "institucion",
                table: "titulos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "titulos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "nombre_titulo",
                table: "titulos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "regiones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "regiones",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "provincias",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "provincias",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "generos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "generos",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "parentesco",
                table: "familiares",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "familiares",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "familiares",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "apellido",
                table: "familiares",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "etnias",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "etnias",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "estados_civiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "estados_civiles",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "datos_bancarios",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_cuenta",
                table: "datos_bancarios",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "numero_cuenta",
                table: "datos_bancarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "telefono",
                table: "contactos_emergencia",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "parentesco",
                table: "contactos_emergencia",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "contactos_emergencia",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "contactos_emergencia",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "direccion",
                table: "contactos_emergencia",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "apellido",
                table: "contactos_emergencia",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nombre",
                table: "ciudades",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "estado",
                table: "ciudades",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "id_usuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "id_rol");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cargos",
                table: "cargos",
                column: "id_cargo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bancos",
                table: "bancos",
                column: "id_banco");

            migrationBuilder.AddPrimaryKey(
                name: "PK_areas",
                table: "areas",
                column: "id_area");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vacaciones",
                table: "vacaciones",
                column: "id_vacacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_titulos",
                table: "titulos",
                column: "id_titulo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_regiones",
                table: "regiones",
                column: "id_region");

            migrationBuilder.AddPrimaryKey(
                name: "PK_provincias",
                table: "provincias",
                column: "id_provincia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_generos",
                table: "generos",
                column: "id_genero");

            migrationBuilder.AddPrimaryKey(
                name: "PK_familiares",
                table: "familiares",
                column: "id_familiar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_etnias",
                table: "etnias",
                column: "id_etnia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_estados_civiles",
                table: "estados_civiles",
                column: "id_estado_civil");

            migrationBuilder.AddPrimaryKey(
                name: "PK_datos_bancarios",
                table: "datos_bancarios",
                column: "id_dato_bancario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contactos_emergencia",
                table: "contactos_emergencia",
                column: "id_contacto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ciudades",
                table: "ciudades",
                column: "id_ciudad");

            migrationBuilder.CreateTable(
                name: "imagenes",
                columns: table => new
                {
                    id_imagen = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ruta_imagen = table.Column<string>(type: "text", nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagenes", x => x.id_imagen);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_cedula",
                table: "usuarios",
                column: "cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo_empresa",
                table: "usuarios",
                column: "correo_empresa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_cargo",
                table: "usuarios",
                column: "id_cargo");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_ciudad",
                table: "usuarios",
                column: "id_ciudad");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_estado_civil",
                table: "usuarios",
                column: "id_estado_civil");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_etnia",
                table: "usuarios",
                column: "id_etnia");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_genero",
                table: "usuarios",
                column: "id_genero");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_rol",
                table: "usuarios",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_usuario",
                table: "usuarios",
                column: "usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cargo_area",
                table: "cargos",
                columns: new[] { "id_area", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bancos_nombre",
                table: "bancos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_areas_nombre",
                table: "areas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_titulos_id_usuario",
                table: "titulos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_regiones_nombre",
                table: "regiones",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_provincia_region",
                table: "provincias",
                columns: new[] { "id_region", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_generos_nombre",
                table: "generos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_familiares_id_usuario",
                table: "familiares",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_etnias_nombre",
                table: "etnias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estados_civiles_nombre",
                table: "estados_civiles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datos_bancarios_id_banco",
                table: "datos_bancarios",
                column: "id_banco");

            migrationBuilder.CreateIndex(
                name: "IX_datos_bancarios_id_usuario",
                table: "datos_bancarios",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_contactos_emergencia_id_usuario",
                table: "contactos_emergencia",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "uq_ciudad_provincia",
                table: "ciudades",
                columns: new[] { "id_provincia", "nombre" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_cargo_area",
                table: "cargos",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id_area",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ciudad_provincia",
                table: "ciudades",
                column: "id_provincia",
                principalTable: "provincias",
                principalColumn: "id_provincia",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contactos_emergencia_usuarios_id_usuario",
                table: "contactos_emergencia",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_datos_bancarios_bancos_id_banco",
                table: "datos_bancarios",
                column: "id_banco",
                principalTable: "bancos",
                principalColumn: "id_banco",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_datos_bancarios_usuarios_id_usuario",
                table: "datos_bancarios",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_familiares_usuarios_id_usuario",
                table: "familiares",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_provincia_region",
                table: "provincias",
                column: "id_region",
                principalTable: "regiones",
                principalColumn: "id_region",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_titulos_usuarios_id_usuario",
                table: "titulos",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_cargos_id_cargo",
                table: "usuarios",
                column: "id_cargo",
                principalTable: "cargos",
                principalColumn: "id_cargo",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_ciudades_id_ciudad",
                table: "usuarios",
                column: "id_ciudad",
                principalTable: "ciudades",
                principalColumn: "id_ciudad",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_estados_civiles_id_estado_civil",
                table: "usuarios",
                column: "id_estado_civil",
                principalTable: "estados_civiles",
                principalColumn: "id_estado_civil",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_etnias_id_etnia",
                table: "usuarios",
                column: "id_etnia",
                principalTable: "etnias",
                principalColumn: "id_etnia",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_generos_id_genero",
                table: "usuarios",
                column: "id_genero",
                principalTable: "generos",
                principalColumn: "id_genero",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_roles_id_rol",
                table: "usuarios",
                column: "id_rol",
                principalTable: "roles",
                principalColumn: "id_rol",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vacaciones_usuarios_id_registrado_por",
                table: "vacaciones",
                column: "id_registrado_por",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vacaciones_usuarios_id_usuario",
                table: "vacaciones",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
