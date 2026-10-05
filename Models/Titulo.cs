using System;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Titulo
{
    [Key]
    public long IdTitulo { get; set; }
    public long IdUsuario { get; set; }
    public string NombreTitulo { get; set; } = null!;
    public string? Institucion { get; set; }
    public DateTime? FechaObtencion { get; set; }

    // true = título culminado (tiene fecha de obtención); false = sigue estudiando (en curso).
    public bool Culminado { get; set; } = true;

    public bool Estado { get; set; } = true;

    public virtual Usuario Usuario { get; set; } = null!;
}