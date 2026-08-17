using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Rol
{
    [Key]
    public long IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}