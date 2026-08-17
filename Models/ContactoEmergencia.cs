namespace Intranet.Models;

using System.ComponentModel.DataAnnotations;
public class ContactoEmergencia
{
    [Key]
    public long IdContacto { get; set; }
    public long IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool Estado { get; set; } = true;

    public virtual Usuario Usuario { get; set; } = null!;
}