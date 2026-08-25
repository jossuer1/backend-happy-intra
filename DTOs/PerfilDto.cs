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
    public string? Cedula { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? CorreoEmpresa { get; set; }
    public string? CorreoPersonal { get; set; }
    public string? CelularPersonal { get; set; }
    public string? CelularEmpresa { get; set; }
    public string? Direccion { get; set; }
    public string? UrlImagenPerfil { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }
    public bool TieneVacaciones { get; set; }
    public int DiasVacacionesAsignados { get; set; }
    public bool Estado { get; set; }

    // --- AGREGAR ESTAS PROPIEDADES DE IDs ---
    public long? IdCargo { get; set; }
    public long? IdCiudad { get; set; }
    public long? IdGenero { get; set; }
    public long? IdEstadoCivil { get; set; }
    public long? IdEtnia { get; set; }

    // Propiedades de texto (para lectura simple si se necesitan)
    public string? Rol { get; set; }
    public string? Cargo { get; set; }
    public string? Departamento { get; set; }
    public string? Ciudad { get; set; }
    public string? Genero { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Etnia { get; set; }

    // Listas de subrecursos...
    public List<FamiliarDto> Familiares { get; set; } = new();
    public List<ContactoEmergenciaDto> ContactosEmergencia { get; set; } = new();
    public List<DatoBancarioDto> DatosBancarios { get; set; } = new();
    public List<TituloDto> Titulos { get; set; } = new();
}