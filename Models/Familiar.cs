using System;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Familiar
{
    [Key]
    public long IdFamiliar { get; set; }
    public long IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public DateTime? FechaNacimiento { get; set; }

    // Solo se usa cuando el parentesco es CONYUGE: desde cuándo son pareja.
    // En ese caso se llena esta fecha y FechaNacimiento queda vacía.
    public DateTime? FechaUnion { get; set; }
    public bool Estado { get; set; } = true;

    public virtual Usuario Usuario { get; set; } = null!;
}