using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Provincia
{
    [Key]
    public long IdProvincia { get; set; }
    public long IdRegion { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual Region Region { get; set; } = null!;
    public virtual ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
}