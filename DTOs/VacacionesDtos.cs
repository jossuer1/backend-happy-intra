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
