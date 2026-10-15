using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

public class RolReadDto
{
    public long IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; }
}

public class RolCrearDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;
}

public class RolActualizarDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;
}

public class AsignarRolUsuarioDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe indicar un usuario válido.")]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe indicar un rol válido.")]
    public long IdRol { get; set; }
}

// Fila de la lista que usa el ADMIN para asignar roles.
public class UsuarioRolDto
{
    public long IdUsuario { get; set; }
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string CorreoEmpresa { get; set; } = null!;
    public string? Cargo { get; set; }
    public long? IdRol { get; set; }
    public string? Rol { get; set; }
    public bool Estado { get; set; }
}
