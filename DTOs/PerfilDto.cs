namespace Intranet.DTOs;

public class FamiliarDto
{
    public long IdFamiliar { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

public class ContactoEmergenciaDto
{
    public long IdContacto { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}

public class DatoBancarioDto
{
    public long IdDatoBancario { get; set; }
    public long IdBanco { get; set; }
    public string Banco { get; set; } = null!;
    public string TipoCuenta { get; set; } = null!;
    public string NumeroCuenta { get; set; } = null!;
}

public class TituloDto
{
    public long IdTitulo { get; set; }
    public string NombreTitulo { get; set; } = null!;
    public string? Institucion { get; set; }
    public DateTime? FechaObtencion { get; set; }
}

public class PerfilDto
{
    public long IdUsuario { get; set; }
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Nombres => $"{Nombre} {Apellido}".Trim();

    public string CorreoEmpresa { get; set; } = null!;
    public string CorreoPersonal { get; set; } = null!;
    public string? CelularPersonal { get; set; }
    public string? CelularEmpresa { get; set; }
    public string? Direccion { get; set; }
    public string? UrlImagenPerfil { get; set; }

    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public int DiasVacacionesAsignados { get; set; }
    public bool Estado { get; set; }

    public string? Rol { get; set; }
    public string? Cargo { get; set; }
    public string? Departamento { get; set; } // Cargo.Area.Nombre
    public string? Ciudad { get; set; }
    public string? Genero { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Etnia { get; set; }

    public List<FamiliarDto> Familiares { get; set; } = new();
    public List<ContactoEmergenciaDto> ContactosEmergencia { get; set; } = new();
    public List<DatoBancarioDto> DatosBancarios { get; set; } = new();
    public List<TituloDto> Titulos { get; set; } = new();
}