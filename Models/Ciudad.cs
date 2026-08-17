using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Ciudad
{
    [Key]
    public long IdCiudad { get; set; }
    public long IdProvincia { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual Provincia Provincia { get; set; } = null!;
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}