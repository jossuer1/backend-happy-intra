using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Region
{
    [Key]
    public long IdRegion { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
}