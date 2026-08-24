using System.ComponentModel.DataAnnotations;

namespace Intranet.DTOs;

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

    public long? IdArea { get; set; }

    [Required(ErrorMessage = "El cargo es obligatorio.")]
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
}