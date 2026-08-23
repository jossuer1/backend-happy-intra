using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;

namespace Intranet.Services;

public class ImagenService : IImagenService
{
    private readonly AppDbContext _context;

    public ImagenService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<ImagenDto>>> ObtenerActivasAsync()
    {
        var imagenes = await _context.Imagen
            .Where(i => i.Estado)
            .OrderBy(i => i.Orden)
            .ThenByDescending(i => i.FechaCreacion)
            .Select(i => new ImagenDto
            {
                IdImagen = i.IdImagen,
                Titulo = i.Titulo,
                Descripcion = i.Descripcion,
                RutaImagen = i.RutaImagen,
                Orden = i.Orden,
                Estado = i.Estado,
                FechaCreacion = i.FechaCreacion
            })
            .ToListAsync();

        return ServiceResult<List<ImagenDto>>.Ok(imagenes);
    }

    public async Task<ServiceResult<List<ImagenDto>>> ObtenerTodasAsync()
    {
        var imagenes = await _context.Imagen
            .OrderBy(i => i.Orden)
            .Select(i => new ImagenDto
            {
                IdImagen = i.IdImagen,
                Titulo = i.Titulo,
                Descripcion = i.Descripcion,
                RutaImagen = i.RutaImagen,
                Orden = i.Orden,
                Estado = i.Estado,
                FechaCreacion = i.FechaCreacion
            })
            .ToListAsync();

        return ServiceResult<List<ImagenDto>>.Ok(imagenes);
    }

    public async Task<ServiceResult<ImagenDto>> AgregarAsync(ImagenCrearDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RutaImagen))
            return ServiceResult<ImagenDto>.Fallo("La ruta/URL de la imagen es obligatoria.");

        var nuevaImagen = new Imagen
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            RutaImagen = dto.RutaImagen,
            Orden = dto.Orden,
            Estado = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        _context.Imagen.Add(nuevaImagen);
        await _context.SaveChangesAsync();

        return ServiceResult<ImagenDto>.Ok(MapearDto(nuevaImagen));
    }

    public async Task<ServiceResult<ImagenDto>> ActualizarAsync(long id, ImagenActualizarDto dto)
    {
        var imagen = await _context.Imagen.FindAsync(id);
        if (imagen == null)
            return ServiceResult<ImagenDto>.Fallo("La imagen no existe.");

        imagen.Titulo = dto.Titulo;
        imagen.Descripcion = dto.Descripcion;
        imagen.RutaImagen = dto.RutaImagen;
        imagen.Orden = dto.Orden;
        imagen.Estado = dto.Estado;
        imagen.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<ImagenDto>.Ok(MapearDto(imagen));
    }

    public async Task<ServiceResult<bool>> CambiarEstadoAsync(long id, bool estado)
    {
        var imagen = await _context.Imagen.FindAsync(id);
        if (imagen == null)
            return ServiceResult<bool>.Fallo("La imagen no existe.");

        imagen.Estado = estado;
        imagen.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private static ImagenDto MapearDto(Imagen i) => new()
    {
        IdImagen = i.IdImagen,
        Titulo = i.Titulo,
        Descripcion = i.Descripcion,
        RutaImagen = i.RutaImagen,
        Orden = i.Orden,
        Estado = i.Estado,
        FechaCreacion = i.FechaCreacion
    };
}