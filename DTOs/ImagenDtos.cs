using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

public class ImagenDto
{
    public long IdImagen { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string RutaImagen { get; set; } = null!;
    public int Orden { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ImagenCrearDto
{
    [Required]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2)]
    public string Titulo { get; set; } = null!;
    
    [StringLength(500)]
    public string? Descripcion { get; set; }
    
    [Range(0, 10000)]
    public int Orden { get; set; } = 0;

    [Required]
    public IFormFile Archivo { get; set; } = null!; 
}

public class ImagenActualizarDto
{
    [Required]
    [NoSoloEspacios]
    [StringLength(100, MinimumLength = 2)]
    public string Titulo { get; set; } = null!;
    [StringLength(500)]
    public string? Descripcion { get; set; }
    public string RutaImagen { get; set; } = null!;
    [Range(0, 10000)]
    public int Orden { get; set; }
    public bool Estado { get; set; }
}
