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
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string RutaImagen { get; set; } = null!;
    public int Orden { get; set; } = 0;
}

public class ImagenActualizarDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string RutaImagen { get; set; } = null!;
    public int Orden { get; set; }
    public bool Estado { get; set; }
}