using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

// --- Genérico: usado por catálogos simples (Áreas, Cargos, Bancos) que solo tienen Nombre ---
public class CatalogoNombreDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = null!;
}

// --- CIUDAD ---
public class CiudadCrearDto
{
    [Required(ErrorMessage = "El nombre de la ciudad es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la ciudad debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La provincia es obligatoria.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe seleccionar una provincia válida.")]
    public long IdProvincia { get; set; }
}

public class CiudadReadDto
{
    public long IdCiudad { get; set; }
    public string Nombre { get; set; } = null!;
    public long IdProvincia { get; set; }
    public bool Estado { get; set; }
    public ProvinciaSimpleDto? Provincia { get; set; }
}
public class ProvinciaSimpleDto
{
    public long IdProvincia { get; set; }
    public string Nombre { get; set; } = null!;
    public long IdRegion { get; set; }
}

public class CiudadActualizarDto
{
    [Required(ErrorMessage = "El nombre de la ciudad es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la ciudad debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La provincia es obligatoria.")]
    [Range(1, long.MaxValue, ErrorMessage = "Debe seleccionar una provincia válida.")]
    public long IdProvincia { get; set; }

    public bool Estado { get; set; } = true;
}

// --- ETNIA ---
public class EtniaCrearDto
{
    [Required(ErrorMessage = "El nombre de la etnia es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre de la etnia debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;
}

public class EtniaActualizarDto
{
    [Required(ErrorMessage = "El nombre de la etnia es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre de la etnia debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;
}

// --- GENERO ---
public class GeneroCrearDto
{
    [Required(ErrorMessage = "El nombre del género es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre del género debe tener entre 2 y 30 caracteres.")]
    public string Nombre { get; set; } = null!;
}

public class GeneroActualizarDto
{
    [Required(ErrorMessage = "El nombre del género es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre del género debe tener entre 2 y 30 caracteres.")]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;
}

// --- ESTADO CIVIL ---
public class EstadoCivilCrearDto
{
    [Required(ErrorMessage = "El nombre del estado civil es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre del estado civil debe tener entre 2 y 30 caracteres.")]
    public string Nombre { get; set; } = null!;
}

public class EstadoCivilActualizarDto
{
    [Required(ErrorMessage = "El nombre del estado civil es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre del estado civil debe tener entre 2 y 30 caracteres.")]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;
}

// --- TIPO DE SANGRE ---
// Solo el rol ADMIN puede crear/editar/desactivar valores de este catálogo.
public class TipoSangreCrearDto
{
    [Required(ErrorMessage = "El tipo de sangre es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(5, MinimumLength = 2, ErrorMessage = "El tipo de sangre debe tener entre 2 y 5 caracteres (ej. O+, AB-).")]
    [RegularExpression(@"^(?i:(A|B|AB|O)[+-])$", ErrorMessage = "El tipo de sangre debe ser uno de: A+, A-, B+, B-, AB+, AB-, O+, O-.")]
    public string Nombre { get; set; } = null!;
}

public class TipoSangreActualizarDto
{
    [Required(ErrorMessage = "El tipo de sangre es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(5, MinimumLength = 2, ErrorMessage = "El tipo de sangre debe tener entre 2 y 5 caracteres (ej. O+, AB-).")]
    [RegularExpression(@"^(?i:(A|B|AB|O)[+-])$", ErrorMessage = "El tipo de sangre debe ser uno de: A+, A-, B+, B-, AB+, AB-, O+, O-.")]
    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; } = true;
}

public class TipoSangreReadDto
{
    public long IdTipoSangre { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; }
}
