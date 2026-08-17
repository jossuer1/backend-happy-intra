using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Banco
{
    [Key]
    public long IdBanco { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual ICollection<DatoBancario> DatosBancarios { get; set; } = new List<DatoBancario>();
}