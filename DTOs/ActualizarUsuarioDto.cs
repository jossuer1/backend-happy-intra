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

    // Cédula del familiar (10 dígitos). Opcional.
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula del familiar debe contener únicamente 10 dígitos numéricos.")]
    public string? Cedula { get; set; }

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

    // true = título culminado; false = sigue estudiando (en curso, sin fecha de obtención).
    public bool Culminado { get; set; } = true;

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

public class PeriodoIessActualizarDto
{
    public long? IdPeriodoIess { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso al IESS es obligatoria.")]
    public DateTime? FechaIngreso { get; set; }

    // Null si el período sigue vigente. Solo un período puede quedar abierto.
    public DateTime? FechaSalida { get; set; }

    [StringLength(100, ErrorMessage = "El cargo IESS no puede superar los 100 caracteres.")]
    public string? CargoIess { get; set; }
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

    // Sectorial: enviar texto vacío para borrarlo.
    [StringLength(60, ErrorMessage = "El sectorial no puede superar los 60 caracteres.")]
    public string? Sectorial { get; set; }

    // Ver Nacionalidades en Models/OpcionesFijas.cs.
    [StringLength(30, ErrorMessage = "La nacionalidad no puede superar los 30 caracteres.")]
    public string? Nacionalidad { get; set; }

    public bool? RecibeComisiones { get; set; }
    public bool? AcumulaDecimos { get; set; }

    // Habilita (o no) a este usuario para ser jefe directo de otros.
    public bool? EsJefe { get; set; }

    // --- Subrecursos: alta y edición (upsert por Id) ---
    public List<FamiliarActualizarDto>? Familiares { get; set; }
    public List<ContactoEmergenciaActualizarDto>? ContactosEmergencia { get; set; }
    public List<TituloActualizarDto>? Titulos { get; set; }
    public List<DatoBancarioActualizarDto>? DatosBancarios { get; set; }
    public List<PeriodoIessActualizarDto>? PeriodosIess { get; set; }

    // --- Subrecursos: baja (por Id) ---
    public List<long>? FamiliaresAEliminar { get; set; }
    public List<long>? ContactosEmergenciaAEliminar { get; set; }
    public List<long>? TitulosAEliminar { get; set; }
    public List<long>? DatosBancariosAEliminar { get; set; }
    public List<long>? PeriodosIessAEliminar { get; set; }
}

// RRHH usa este DTO para habilitar/deshabilitar la ventana de autoedición
// de perfil de un empleado (PATCH /api/Usuarios/{id}/permiso-actualizacion).
// Al habilitar, FechaLimite es obligatoria: el empleado puede enviar sus cambios
// hasta el final de ese día (hora de Ecuador).
public class PermisoActualizarPerfilDto
{
    public bool Habilitar { get; set; }

    public DateTime? FechaLimite { get; set; }
}

// RRHH habilita la ventana a varios empleados de una sola vez
// (POST /api/Usuarios/habilitar-actualizacion). O bien TodosActivos = true,
// o bien una lista de IdsUsuarios (nunca ambos vacíos). Solo afecta a usuarios activos.
public class HabilitarActualizacionMasivaDto
{
    public bool TodosActivos { get; set; }

    public List<long>? IdsUsuarios { get; set; }

    [Required(ErrorMessage = "La fecha límite es obligatoria.")]
    public DateTime? FechaLimite { get; set; }
}

public class HabilitarActualizacionMasivaResultadoDto
{
    public int Habilitados { get; set; }

    // Activos que se omitieron porque ya tienen una solicitud pendiente de revisión.
    public int OmitidosPorSolicitudPendiente { get; set; }

    public DateTime FechaLimite { get; set; }
}

// RRHH envía un recordatorio a quienes tienen la ventana abierta y aún no respondieron
// (POST /api/Usuarios/recordatorio-actualizacion). Opcionalmente amplía la fecha de referencia.
public class RecordatorioActualizacionDto
{
    // true = a todos los activos con la ventana abierta; false = solo a IdsUsuarios.
    public bool TodosSinEnviar { get; set; }

    public List<long>? IdsUsuarios { get; set; }

    // Opcional. Si viene, reemplaza la fecha de referencia de los destinatarios
    // (no puede ser anterior a hoy).
    public DateTime? NuevaFechaLimite { get; set; }
}

public class RecordatorioActualizacionResultadoDto
{
    public int Enviados { get; set; }

    // De los ids indicados, los que no tenían la ventana abierta (ya respondieron,
    // están inactivos, etc.). Con TodosSinEnviar siempre es 0.
    public int Omitidos { get; set; }

    // Solo viene si se amplió la fecha.
    public DateTime? FechaLimite { get; set; }
}

// RRHH usa este DTO para activar/desactivar la cuenta de un empleado
// (PATCH /api/Usuarios/{id}/estado). Un usuario desactivado no puede
// iniciar sesión ni cambiar su contraseña (ver AuthService).
public class ActualizarEstadoUsuarioDto
{
    public bool Activar { get; set; }
}

// Subconjunto de datos que el propio empleado puede proponer cuando RRHH le
// habilita la ventana de edición (PUT /api/Usuarios/mi-perfil). Los cambios NO se
// aplican al instante: quedan como solicitud pendiente hasta que RRHH los apruebe.
// Deliberadamente NO incluye cédula, nombre, cargo, ciudad, rol, tipo de sangre,
// datos bancarios ni nada laboral: eso sigue siendo exclusivo de RRHH vía
// PUT /api/Usuarios/{id}. La foto tampoco va aquí: se sube al instante con
// POST /api/Usuarios/{id}/foto.
// Regla de los campos escalares: null = "no cambiar".
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

    [EmailAddress(ErrorMessage = "El formato del correo personal no es válido.")]
    [StringLength(100, ErrorMessage = "El correo personal no puede superar los 100 caracteres.")]
    public string? CorreoPersonal { get; set; }

    public long? IdEstadoCivil { get; set; }

    // --- Subrecursos que también puede mantener el propio empleado ---
    public List<FamiliarActualizarDto>? Familiares { get; set; }
    public List<ContactoEmergenciaActualizarDto>? ContactosEmergencia { get; set; }
    public List<TituloActualizarDto>? Titulos { get; set; }

    public List<long>? FamiliaresAEliminar { get; set; }
    public List<long>? ContactosEmergenciaAEliminar { get; set; }
    public List<long>? TitulosAEliminar { get; set; }
}
