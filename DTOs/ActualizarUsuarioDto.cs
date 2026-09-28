using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

// ---------------------------------------------------------------------
// Subrecursos dentro del PUT completo de usuario (solo RRHH).
// Regla: si el Id viene null o 0 => se crea un registro nuevo.
//        si el Id viene con un valor existente => se edita ese registro
//        (debe pertenecer al usuario que se está actualizando).
// La baja se maneja aparte, con las listas "...AEliminar" (por Id).
// ---------------------------------------------------------------------

public class FamiliarActualizarDto : IFamiliarEntrada
{
    public long? IdFamiliar { get; set; }

    [Required(ErrorMessage = "El nombre del familiar es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? Apellido { get; set; }

    // Valores permitidos: CONYUGE o HIJO (ver Parentescos).
    [StringLength(30)]
    public string? Parentesco { get; set; }

    // Para HIJO. Si el parentesco es CONYUGE se ignora (se usa FechaUnion).
    public DateTime? FechaNacimiento { get; set; }

    // Solo para CONYUGE: desde cuándo son pareja.
    public DateTime? FechaUnion { get; set; }
}

public class ContactoEmergenciaActualizarDto
{
    public long? IdContacto { get; set; }

    [Required(ErrorMessage = "El nombre del contacto es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? Apellido { get; set; }

    [StringLength(30)]
    public string? Parentesco { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Direccion { get; set; }
}

public class TituloActualizarDto
{
    public long? IdTitulo { get; set; }

    [Required(ErrorMessage = "El nombre del título es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2)]
    public string NombreTitulo { get; set; } = null!;

    [StringLength(100)]
    public string? Institucion { get; set; }

    public DateTime? FechaObtencion { get; set; }
}

public class DatoBancarioActualizarDto
{
    public long? IdDatoBancario { get; set; }

    [Required(ErrorMessage = "El banco es obligatorio.")]
    public long IdBanco { get; set; }

    [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(20, MinimumLength = 3)]
    public string TipoCuenta { get; set; } = null!;

    [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
    [NoSoloEspacios]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de cuenta solo puede contener dígitos.")]
    [StringLength(30, MinimumLength = 3)]
    public string NumeroCuenta { get; set; } = null!;
}

public class ActualizarUsuarioDto
{
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string? Nombre { get; set; }

    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string? Apellido { get; set; }

    [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener exactamente 10 dígitos.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe contener únicamente 10 dígitos numéricos.")]
    public string? Cedula { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo empresarial no es válido.")]
    [StringLength(100, ErrorMessage = "El correo empresarial no puede superar los 100 caracteres.")]
    public string? CorreoEmpresa { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo personal no es válido.")]
    [StringLength(100, ErrorMessage = "El correo personal no puede superar los 100 caracteres.")]
    public string? CorreoPersonal { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public long? IdGenero { get; set; }

    public long? IdEstadoCivil { get; set; }

    public long? IdEtnia { get; set; }

    public long? IdCargo { get; set; }

    public long? IdCiudad { get; set; }

    public long? IdTipoSangre { get; set; }

    public long? IdJefeDirecto { get; set; }

    public DateTime? FechaIngreso { get; set; }

    [StringLength(20, ErrorMessage = "El celular empresarial no es válido.")]
    public string? CelularEmpresa { get; set; }

    [StringLength(20, ErrorMessage = "El celular personal no es válido.")]
    public string? CelularPersonal { get; set; }

    [StringLength(150, ErrorMessage = "La dirección es demasiado larga.")]
    public string? Direccion { get; set; }

    public bool? TieneVacaciones { get; set; }

    public int? DiasVacacionesAsignados { get; set; }

    // --- Condición laboral (solo se tocan los que vienen informados) ---
    // CargoIess: enviar texto vacío para borrarlo.
    [StringLength(100, ErrorMessage = "El cargo IESS no puede superar los 100 caracteres.")]
    public string? CargoIess { get; set; }

    [StringLength(30, ErrorMessage = "La jornada no puede superar los 30 caracteres.")]
    public string? Jornada { get; set; }

    [StringLength(30, ErrorMessage = "El tipo de contrato no puede superar los 30 caracteres.")]
    public string? TipoContrato { get; set; }

    // Obligatoria (y solo permitida) para contratos EMERGENTE o PRODUCTIVO. Si el
    // contrato pasa a otro tipo, la fecha guardada se borra sola.
    public DateTime? FechaFinContrato { get; set; }

    public bool? RecibeComisiones { get; set; }
    public bool? AcumulaDecimos { get; set; }

    // Habilita (o no) a este usuario para ser jefe directo de otros.
    public bool? EsJefe { get; set; }

    // --- Subrecursos: alta y edición (upsert por Id) ---
    public List<FamiliarActualizarDto>? Familiares { get; set; }
    public List<ContactoEmergenciaActualizarDto>? ContactosEmergencia { get; set; }
    public List<TituloActualizarDto>? Titulos { get; set; }
    public List<DatoBancarioActualizarDto>? DatosBancarios { get; set; }

    // --- Subrecursos: baja (por Id) ---
    public List<long>? FamiliaresAEliminar { get; set; }
    public List<long>? ContactosEmergenciaAEliminar { get; set; }
    public List<long>? TitulosAEliminar { get; set; }
    public List<long>? DatosBancariosAEliminar { get; set; }
}

// RRHH usa este DTO para habilitar/deshabilitar la ventana de autoedición
// de perfil de un empleado (PATCH /api/Usuarios/{id}/permiso-actualizacion).
public class PermisoActualizarPerfilDto
{
    public bool Habilitar { get; set; }
}

// RRHH usa este DTO para activar/desactivar la cuenta de un empleado
// (PATCH /api/Usuarios/{id}/estado). Un usuario desactivado no puede
// iniciar sesión ni cambiar su contraseña (ver AuthService).
public class ActualizarEstadoUsuarioDto
{
    public bool Activar { get; set; }
}

// Subconjunto de datos que el propio empleado puede editar cuando RRHH le
// habilita el permiso (PUT /api/Usuarios/mi-perfil). Deliberadamente NO
// incluye cédula, correos, cargo, ciudad, rol ni beneficio de vacaciones:
// esos campos siguen siendo exclusivos de RRHH vía el PUT /api/Usuarios/{id}.
public class ActualizarPerfilPropioDto
{
    [Phone(ErrorMessage = "El celular personal no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El celular personal no puede superar los 20 caracteres.")]
    public string? CelularPersonal { get; set; }

    [Phone(ErrorMessage = "El celular empresarial no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El celular empresarial no puede superar los 20 caracteres.")]
    public string? CelularEmpresa { get; set; }

    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string? Direccion { get; set; }

    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string? UrlImagenPerfil { get; set; }

    // --- Subrecursos que también puede mantener el propio empleado ---
    public List<FamiliarActualizarDto>? Familiares { get; set; }
    public List<ContactoEmergenciaActualizarDto>? ContactosEmergencia { get; set; }

    public List<long>? FamiliaresAEliminar { get; set; }
    public List<long>? ContactosEmergenciaAEliminar { get; set; }
}
