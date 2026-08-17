using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Area
{
    [Key]
    public long IdArea { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; } = true;

    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
}