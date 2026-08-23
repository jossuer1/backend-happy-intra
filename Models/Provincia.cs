using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models;

public class Provincia
{
    [Key]
    public long IdProvincia { get; set; }

    public long IdRegion { get; set; }

    [Required]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;

    [ForeignKey("IdRegion")]
    public virtual Region Region { get; set; } = null!;

    public virtual ICollection<Ciudad> Ciudades { get; set; } = new List<Ciudad>();
}