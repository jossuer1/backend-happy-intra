namespace Intranet.DTOs;


public class FamiliarCrearDto
{
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

public class ContactoEmergenciaCrearDto
{
    public string Nombre { get; set; } = null!;
    public string? Apellido { get; set; }
    public string? Parentesco { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}

public class DatoBancarioCrearDto
{
    public long IdBanco { get; set; }
    public string TipoCuenta { get; set; } = null!;
    public string NumeroCuenta { get; set; } = null!;
}

public class TituloCrearDto
{
    public string NombreTitulo { get; set; } = null!;
    public string Institucion { get; set; } = null!;
}

public class CrearUsuarioDto
{
    // Datos principales
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string CorreoEmpresa { get; set; } = null!;
    public string CorreoPersonal { get; set; } = null!;

    // Claves foráneas (sin IdRol)
    public long? IdCargo { get; set; }
    public long? IdCiudad { get; set; }
    public long? IdEstadoCivil { get; set; }
    public long? IdEtnia { get; set; }
    public long? IdGenero { get; set; }

    // Información personal
    public string? CelularPersonal { get; set; }
    public string? CelularEmpresa { get; set; }
    public string? Direccion { get; set; }
    public string? UrlImagenPerfil { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }

    // Beneficio de vacaciones: por defecto todo usuario nuevo lo tiene habilitado.
    // Si TieneVacaciones = false, se ignora DiasVacacionesAsignados y se guarda en 0.
    public bool TieneVacaciones { get; set; } = true;
    public int? DiasVacacionesAsignados { get; set; }

    // Listas anidadas
    public List<FamiliarCrearDto>? Familiares { get; set; }
    public List<ContactoEmergenciaCrearDto>? ContactosEmergencia { get; set; }
    public List<DatoBancarioCrearDto>? DatosBancarios { get; set; }
    public List<TituloCrearDto>? Titulos { get; set; }
}