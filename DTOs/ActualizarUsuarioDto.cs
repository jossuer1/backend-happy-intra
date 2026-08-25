using System.ComponentModel.DataAnnotations;

namespace Intranet.DTOs;

// ---------------------------------------------------------------------
// Subrecursos dentro del PUT completo de usuario (solo RRHH).
// Regla: si el Id viene null o 0 => se crea un registro nuevo.
//        si el Id viene con un valor existente => se edita ese registro
//        (debe pertenecer al usuario que se está actualizando).
// La baja se maneja aparte, con las listas "...AEliminar" (por Id).
// ---------------------------------------------------------------------

public class FamiliarActualizarDto
{
    public long? IdFamiliar { get; set; }

    [Required(ErrorMessage = "El nombre del familiar es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? Apellido { get; set; }

    [StringLength(30)]
    public string? Parentesco { get; set; }

    public DateTime? FechaNacimiento { get; set; }
}

public class ContactoEmergenciaActualizarDto
{
    public long? IdContacto { get; set; }

    [Required(ErrorMessage = "El nombre del contacto es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? Apellido { get; set; }

    [StringLength(30)]
    public string? Parentesco { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Direccion { get; set; }
}

public class TituloActualizarDto
{
    public long? IdTitulo { get; set; }

    [Required(ErrorMessage = "El nombre del título es obligatorio.")]
    [StringLength(100)]
    public string NombreTitulo { get; set; } = null!;

    [StringLength(100)]
    public string? Institucion { get; set; }

    public DateTime? FechaObtencion { get; set; }
}

public class DatoBancarioActualizarDto
{
    public long? IdDatoBancario { get; set; }

    [Required(ErrorMessage = "El banco es obligatorio.")]
    public long IdBanco { get; set; }

    [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
    [StringLength(20)]
    public string TipoCuenta { get; set; } = null!;

    [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
    [StringLength(30)]
    public string NumeroCuenta { get; set; } = null!;
}

public class ActualizarUsuarioDto
{
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string? Nombre { get; set; }

    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
    public string? Apellido { get; set; }

    [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener exactamente 10 dígitos.")]
    public string? Cedula { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo empresarial no es válido.")]
    public string? CorreoEmpresa { get; set; }

    [EmailAddress(ErrorMessage = "El formato del correo personal no es válido.")]
    public string? CorreoPersonal { get; set; }

    public DateTime? FechaNacimiento { get; set; }

    public long? IdGenero { get; set; }

    public long? IdEstadoCivil { get; set; }

    public long? IdEtnia { get; set; }

    public long? IdCargo { get; set; }

    public long? IdCiudad { get; set; }

    public DateTime? FechaIngreso { get; set; }

    [StringLength(20, ErrorMessage = "El celular empresarial no es válido.")]
    public string? CelularEmpresa { get; set; }

    [StringLength(20, ErrorMessage = "El celular personal no es válido.")]
    public string? CelularPersonal { get; set; }

    [StringLength(150, ErrorMessage = "La dirección es demasiado larga.")]
    public string? Direccion { get; set; }

    public bool? TieneVacaciones { get; set; }

    public int? DiasVacacionesAsignados { get; set; }

    // --- Subrecursos: alta y edición (upsert por Id) ---
    public List<FamiliarActualizarDto>? Familiares { get; set; }
    public List<ContactoEmergenciaActualizarDto>? ContactosEmergencia { get; set; }
    public List<TituloActualizarDto>? Titulos { get; set; }
    public List<DatoBancarioActualizarDto>? DatosBancarios { get; set; }

    // --- Subrecursos: baja (por Id) ---
    public List<long>? FamiliaresAEliminar { get; set; }
    public List<long>? ContactosEmergenciaAEliminar { get; set; }
    public List<long>? TitulosAEliminar { get; set; }
    public List<long>? DatosBancariosAEliminar { get; set; }
}
