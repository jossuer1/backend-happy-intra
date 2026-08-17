using System;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Imagen
{
    [Key]
    public long IdImagen { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string RutaImagen { get; set; } = null!;
    public int Orden { get; set; } = 0;
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}