namespace Intranet.DTOs;

public class RolReadDto
{
    public long IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; }
}

public class RolCrearDto
{
    public string Nombre { get; set; } = null!;
}

public class RolActualizarDto
{
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;
}

public class AsignarRolUsuarioDto
{
    public long IdUsuario { get; set; }
    public long IdRol { get; set; }
}