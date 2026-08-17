using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Etnia
{
    [Key]
    public long IdEtnia { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}