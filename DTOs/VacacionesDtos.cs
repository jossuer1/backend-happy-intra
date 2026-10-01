using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

// Registrar vacaciones tomadas (descuenta días según el rango de fechas, calendario completo)
public class VacacionDescuentoCrearDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe indicar un usuario válido.")]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres.")]
    public string Motivo { get; set; } = null!;
}

// Corrección manual: devuelve días sin necesidad de un rango de fechas
public class VacacionAjusteCrearDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe indicar un usuario válido.")]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "Los días son obligatorios.")]
    [Range(-365, 365, ErrorMessage = "Los días deben estar entre -365 y 365.")]
    public int Dias { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres.")]
    public string Motivo { get; set; } = null!;
}

public class VacacionDto
{
    public long IdVacacion { get; set; }
    public string TipoMovimiento { get; set; } = null!;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int DiasTomados { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string RegistradoPorNombre { get; set; } = null!;

    // Solicitud aprobada que originó este movimiento (null si RRHH lo registró
    // directo o si es un ajuste). Sirve para descargar la constancia en PDF
    // desde el historial: GET /vacaciones/solicitudes/{IdSolicitud}/constancia
    public long? IdSolicitud { get; set; }
}

public class SaldoVacacionesDto
{
    public long IdUsuario { get; set; }
    public int DiasAsignados { get; set; }
    public int DiasDescontados { get; set; }
    public int DiasAjustados { get; set; }
    public int DiasDisponibles { get; set; }
}

// Resumen por empleado para las pantallas de RRHH (Gestión de Vacaciones / Saldos Personal).
// Incluye a TODOS los usuarios activos, incluso los que no tienen el beneficio,
// para que el frontend pueda mostrarlos deshabilitados en vez de que "desaparezcan".
public class ResumenVacacionesDto
{
    public long IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Departamento { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public bool TieneVacaciones { get; set; }
    public int DiasGanados { get; set; }
    public int DiasTomados { get; set; }
    public int SaldoDisponible { get; set; }
}

// Activar/desactivar el beneficio de vacaciones de un usuario existente,
// y opcionalmente ajustar los días asignados.
public class ActualizarVacacionesUsuarioDto
{
    public bool TieneVacaciones { get; set; }

    [Range(0, 365, ErrorMessage = "Los días asignados deben estar entre 0 y 365.")]
    public int? DiasVacacionesAsignados { get; set; }
}

public class VacacionesUsuarioActualizadoDto
{
    public long IdUsuario { get; set; }
    public bool TieneVacaciones { get; set; }
    public int DiasVacacionesAsignados { get; set; }
}