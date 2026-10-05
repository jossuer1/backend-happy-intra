using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Intranet.Models;

// Un tramo de afiliación al IESS de un colaborador: entrada, salida y cargo con el
// que constaba en ese tramo. Un mismo colaborador puede tener varios (entra, sale,
// vuelve a entrar...). El período vigente es el que no tiene FechaSalida.
public class PeriodoIess
{
    [Key]
    public long IdPeriodoIess { get; set; }

    public long IdUsuario { get; set; }

    public DateTime FechaIngreso { get; set; }

    // Null mientras el colaborador siga afiliado en este período.
    public DateTime? FechaSalida { get; set; }

    // Cargo tal como constaba en el IESS durante este período.
    public string? CargoIess { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario Usuario { get; set; } = null!;
}
