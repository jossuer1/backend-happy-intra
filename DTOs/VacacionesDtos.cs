namespace Intranet.DTOs;

// Registrar vacaciones tomadas (descuenta días según el rango de fechas, calendario completo)
public class VacacionDescuentoCrearDto
{
    public long IdUsuario { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Motivo { get; set; } = null!;
}

// Corrección manual: devuelve días sin necesidad de un rango de fechas
public class VacacionAjusteCrearDto
{
    public long IdUsuario { get; set; }
    public int Dias { get; set; }
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
    public int? DiasVacacionesAsignados { get; set; }
}

public class VacacionesUsuarioActualizadoDto
{
    public long IdUsuario { get; set; }
    public bool TieneVacaciones { get; set; }
    public int DiasVacacionesAsignados { get; set; }
}
