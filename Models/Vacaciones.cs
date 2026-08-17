using System;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Vacacion
{
    [Key]
    public long IdVacacion { get; set; }
    public long IdUsuario { get; set; }
    public long IdRegistradoPor { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int DiasTomados { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Estado { get; set; } = true;

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Usuario RegistradoPor { get; set; } = null!;
}