using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Intranet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id_area = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id_area);
                });

            migrationBuilder.CreateTable(
                name: "bancos",
                columns: table => new
                {
                    id_banco = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bancos", x => x.id_banco);
                });

            migrationBuilder.CreateTable(
                name: "estados_civiles",
                columns: table => new
                {
                    id_estado_civil = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_civiles", x => x.id_estado_civil);
                });

            migrationBuilder.CreateTable(
                name: "etnias",
                columns: table => new
                {
                    id_etnia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etnias", x => x.id_etnia);
                });

            migrationBuilder.CreateTable(
                name: "generos",
                columns: table => new
                {
                    id_genero = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generos", x => x.id_genero);
                });

            migrationBuilder.CreateTable(
                name: "imagenes",
                columns: table => new
                {
                    id_imagen = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ruta_imagen = table.Column<string>(type: "text", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagenes", x => x.id_imagen);
                });

            migrationBuilder.CreateTable(
                name: "regiones",
                columns: table => new
                {
                    id_region = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regiones", x => x.id_region);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id_rol = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "cargos",
                columns: table => new
                {
                    id_cargo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_area = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cargos", x => x.id_cargo);
                    table.ForeignKey(
                        name: "fk_cargo_area",
                        column: x => x.id_area,
                        principalTable: "areas",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "provincias",
                columns: table => new
                {
                    id_provincia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_region = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provincias", x => x.id_provincia);
                    table.ForeignKey(
                        name: "fk_provincia_region",
                        column: x => x.id_region,
                        principalTable: "regiones",
                        principalColumn: "id_region",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ciudades",
                columns: table => new
                {
                    id_ciudad = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_provincia = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ciudades", x => x.id_ciudad);
                    table.ForeignKey(
                        name: "fk_ciudad_provincia",
                        column: x => x.id_provincia,
                        principalTable: "provincias",
                        principalColumn: "id_provincia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_rol = table.Column<long>(type: "bigint", nullable: false),
                    id_cargo = table.Column<long>(type: "bigint", nullable: true),
                    id_ciudad = table.Column<long>(type: "bigint", nullable: true),
                    id_estado_civil = table.Column<long>(type: "bigint", nullable: true),
                    id_etnia = table.Column<long>(type: "bigint", nullable: true),
                    id_genero = table.Column<long>(type: "bigint", nullable: true),
                    cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correo_empresa = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    contrasena_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    debe_cambiar_contrasena = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    url_imagen_perfil = table.Column<string>(type: "text", nullable: true),
                    dias_vacaciones_asignados = table.Column<int>(type: "integer", nullable: false, defaultValue: 15),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_usuarios_cargos_id_cargo",
                        column: x => x.id_cargo,
                        principalTable: "cargos",
                        principalColumn: "id_cargo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuarios_ciudades_id_ciudad",
                        column: x => x.id_ciudad,
                        principalTable: "ciudades",
                        principalColumn: "id_ciudad",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuarios_estados_civiles_id_estado_civil",
                        column: x => x.id_estado_civil,
                        principalTable: "estados_civiles",
                        principalColumn: "id_estado_civil",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuarios_etnias_id_etnia",
                        column: x => x.id_etnia,
                        principalTable: "etnias",
                        principalColumn: "id_etnia",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuarios_generos_id_genero",
                        column: x => x.id_genero,
                        principalTable: "generos",
                        principalColumn: "id_genero",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contactos_emergencia",
                columns: table => new
                {
                    id_contacto = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parentesco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactos_emergencia", x => x.id_contacto);
                    table.ForeignKey(
                        name: "FK_contactos_emergencia_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "datos_bancarios",
                columns: table => new
                {
                    id_dato_bancario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    id_banco = table.Column<long>(type: "bigint", nullable: false),
                    numero_cuenta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo_cuenta = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datos_bancarios", x => x.id_dato_bancario);
                    table.ForeignKey(
                        name: "FK_datos_bancarios_bancos_id_banco",
                        column: x => x.id_banco,
                        principalTable: "bancos",
                        principalColumn: "id_banco",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_datos_bancarios_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "familiares",
                columns: table => new
                {
                    id_familiar = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    parentesco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familiares", x => x.id_familiar);
                    table.ForeignKey(
                        name: "FK_familiares_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "titulos",
                columns: table => new
                {
                    id_titulo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    nombre_titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    institucion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    fecha_obtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titulos", x => x.id_titulo);
                    table.ForeignKey(
                        name: "FK_titulos_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vacaciones",
                columns: table => new
                {
                    id_vacacion = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    id_registrado_por = table.Column<long>(type: "bigint", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dias_tomados = table.Column<int>(type: "integer", nullable: false),
                    observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vacaciones", x => x.id_vacacion);
                    table.ForeignKey(
                        name: "FK_vacaciones_usuarios_id_registrado_por",
                        column: x => x.id_registrado_por,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vacaciones_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_areas_nombre",
                table: "areas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bancos_nombre",
                table: "bancos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_cargo_area",
                table: "cargos",
                columns: new[] { "id_area", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_ciudad_provincia",
                table: "ciudades",
                columns: new[] { "id_provincia", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactos_emergencia_id_usuario",
                table: "contactos_emergencia",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_datos_bancarios_id_banco",
                table: "datos_bancarios",
                column: "id_banco");

            migrationBuilder.CreateIndex(
                name: "IX_datos_bancarios_id_usuario",
                table: "datos_bancarios",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_estados_civiles_nombre",
                table: "estados_civiles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etnias_nombre",
                table: "etnias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_familiares_id_usuario",
                table: "familiares",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_generos_nombre",
                table: "generos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_provincia_region",
                table: "provincias",
                columns: new[] { "id_region", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_regiones_nombre",
                table: "regiones",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_titulos_id_usuario",
                table: "titulos",
                column: "id_usuario");

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
                name: "IX_vacaciones_id_registrado_por",
                table: "vacaciones",
                column: "id_registrado_por");

            migrationBuilder.CreateIndex(
                name: "IX_vacaciones_id_usuario",
                table: "vacaciones",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contactos_emergencia");

            migrationBuilder.DropTable(
                name: "datos_bancarios");

            migrationBuilder.DropTable(
                name: "familiares");

            migrationBuilder.DropTable(
                name: "imagenes");

            migrationBuilder.DropTable(
                name: "titulos");

            migrationBuilder.DropTable(
                name: "vacaciones");

            migrationBuilder.DropTable(
                name: "bancos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "cargos");

            migrationBuilder.DropTable(
                name: "ciudades");

            migrationBuilder.DropTable(
                name: "estados_civiles");

            migrationBuilder.DropTable(
                name: "etnias");

            migrationBuilder.DropTable(
                name: "generos");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "provincias");

            migrationBuilder.DropTable(
                name: "regiones");
        }
    }
}
