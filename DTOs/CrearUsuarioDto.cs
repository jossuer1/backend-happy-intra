using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;


public class FamiliarCrearDto : IFamiliarEntrada
{
    [Required(ErrorMessage = "El nombre del familiar es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del familiar debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
    public string? Apellido { get; set; }

    // Valores permitidos: CONYUGE o HIJO (ver Parentescos).
    [StringLength(30, ErrorMessage = "El parentesco no puede superar los 30 caracteres.")]
    public string? Parentesco { get; set; }

    // Para HIJO. Si el parentesco es CONYUGE se ignora (se usa FechaUnion).
    public DateTime? FechaNacimiento { get; set; }

    // Solo para CONYUGE: desde cuándo son pareja.
    public DateTime? FechaUnion { get; set; }
}

public class ContactoEmergenciaCrearDto
{
    [Required(ErrorMessage = "El nombre del contacto es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del contacto debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
    public string? Apellido { get; set; }

    [StringLength(30, ErrorMessage = "El parentesco no puede superar los 30 caracteres.")]
    public string? Parentesco { get; set; }

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Telefono { get; set; }

    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string? Direccion { get; set; }
}

public class DatoBancarioCrearDto
{
    [Required(ErrorMessage = "El banco es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe seleccionar un banco válido.")]
    public long IdBanco { get; set; }

    [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El tipo de cuenta debe tener entre 3 y 20 caracteres.")]
    public string TipoCuenta { get; set; } = null!;

    [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "El número de cuenta debe tener entre 3 y 30 caracteres.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El número de cuenta solo puede contener dígitos.")]
    public string NumeroCuenta { get; set; } = null!;
}

public class TituloCrearDto
{
    [Required(ErrorMessage = "El nombre del título es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del título debe tener entre 2 y 100 caracteres.")]
    public string NombreTitulo { get; set; } = null!;

    [Required(ErrorMessage = "La institución es obligatoria.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "La institución debe tener entre 2 y 100 caracteres.")]
    public string Institucion { get; set; } = null!;
}

public class CrearUsuarioDto
{
    // Datos principales
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener exactamente 10 dígitos.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe contener únicamente 10 dígitos numéricos.")]
    public string Cedula { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El correo empresarial es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo empresarial no es válido.")]
    [StringLength(100, ErrorMessage = "El correo empresarial no puede superar los 100 caracteres.")]
    public string CorreoEmpresa { get; set; } = null!;

    [Required(ErrorMessage = "El correo personal es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo personal no es válido.")]
    [StringLength(100, ErrorMessage = "El correo personal no puede superar los 100 caracteres.")]
    public string CorreoPersonal { get; set; } = null!;

    // Claves foráneas (sin IdRol)
    public long? IdCargo { get; set; }
    public long? IdCiudad { get; set; }
    public long? IdEstadoCivil { get; set; }
    public long? IdEtnia { get; set; }
    public long? IdGenero { get; set; }
    public long? IdTipoSangre { get; set; }

    // Jefe directo: quien aprobará (primer nivel) las solicitudes de vacaciones
    // de este empleado antes de que pasen a RRHH.
    public long? IdJefeDirecto { get; set; }

    // Información personal
    [Phone(ErrorMessage = "El celular personal no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El celular personal no puede superar los 20 caracteres.")]
    public string? CelularPersonal { get; set; }

    [Phone(ErrorMessage = "El celular empresarial no tiene un formato válido.")]
    [StringLength(20, ErrorMessage = "El celular empresarial no puede superar los 20 caracteres.")]
    public string? CelularEmpresa { get; set; }

    [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
    public string? Direccion { get; set; }

    [StringLength(500, ErrorMessage = "La URL de la imagen no puede superar los 500 caracteres.")]
    public string? UrlImagenPerfil { get; set; }

    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public bool TieneVacaciones { get; set; } = true;

    [Range(0, 365, ErrorMessage = "Los días de vacaciones asignados deben estar entre 0 y 365.")]
    public int? DiasVacacionesAsignados { get; set; }

    // Condición laboral
    [StringLength(100, ErrorMessage = "El cargo IESS no puede superar los 100 caracteres.")]
    public string? CargoIess { get; set; }

    // Ver Jornadas en Models/OpcionesFijas.cs
    [StringLength(30, ErrorMessage = "La jornada no puede superar los 30 caracteres.")]
    public string? Jornada { get; set; }

    // Ver TiposContrato en Models/OpcionesFijas.cs
    [StringLength(30, ErrorMessage = "El tipo de contrato no puede superar los 30 caracteres.")]
    public string? TipoContrato { get; set; }

    // Obligatoria (y solo permitida) para contratos EMERGENTE o PRODUCTIVO.
    public DateTime? FechaFinContrato { get; set; }

    public bool RecibeComisiones { get; set; }
    public bool AcumulaDecimos { get; set; }

    // Habilita a este usuario para ser elegido como jefe directo de otros.
    public bool EsJefe { get; set; }

    // Listas anidadas
    public List<FamiliarCrearDto>? Familiares { get; set; }
    public List<ContactoEmergenciaCrearDto>? ContactosEmergencia { get; set; }
    public List<DatoBancarioCrearDto>? DatosBancarios { get; set; }
    public List<TituloCrearDto>? Titulos { get; set; }
}
