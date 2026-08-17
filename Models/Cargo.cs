using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Intranet.Models;

public class Cargo
{
    [Key]
    public long IdCargo { get; set; }
    public long IdArea { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; } = true;
    [ForeignKey(nameof(IdArea))]
    public virtual Area Area { get; set; } = null!;
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}