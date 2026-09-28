using System.ComponentModel.DataAnnotations;

namespace Intranet.Models;

// Catálogo de tipos de sangre (O+, O-, A+, A-, B+, B-, AB+, AB-).
// Solo el rol ADMIN puede crear/editar/desactivar valores de este catálogo
// (ver CatalogosController); cualquier usuario autenticado o no puede leerlo.
public class TipoSangre
{
    [Key]
    public long IdTipoSangre { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
