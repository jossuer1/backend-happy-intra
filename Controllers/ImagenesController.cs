using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Intranet.DTOs;
using Intranet.Services;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagenesController : ControllerBase
{
    private readonly IImagenService _imagenService;

    public ImagenesController(IImagenService imagenService)
    {
        _imagenService = imagenService;
    }

    // 1. Obtener imágenes activas del carrusel (Para todos los usuarios logueados)
    [HttpGet]
    public async Task<IActionResult> ObtenerActivas()
    {
        var resultado = await _imagenService.ObtenerActivasAsync();
        return Ok(resultado.Data);
    }

    // 2. Exclusivo RRHH: Obtener todas las imágenes (activas e inactivas)
    [HttpGet("todas")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _imagenService.ObtenerTodasAsync();
        return Ok(resultado.Data);
    }
    // 3. Exclusivo RRHH: Guardar la nueva imagen subida desde el frontend
    [HttpPost]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Agregar([FromForm] ImagenCrearDto dto) 
    {
    var resultado = await _imagenService.AgregarAsync(dto);

    if (!resultado.Exito)
        return BadRequest(new { mensaje = resultado.Mensaje });

    return Ok(resultado.Data);
    }
    // 4. Exclusivo RRHH: Actualizar datos u orden
    [HttpPut("{id}")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ImagenActualizarDto dto)
    {
        var resultado = await _imagenService.ActualizarAsync(id, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 5. Exclusivo RRHH: Baja lógica
    [HttpDelete("{id}")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Desactivar(long id)
    {
        var resultado = await _imagenService.CambiarEstadoAsync(id, false);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = "Imagen removida del carrusel correctamente." });
    }

    // 6. Exclusivo RRHH: Eliminación definitiva (borra el registro de la base de datos)
    [HttpDelete("{id}/eliminar")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var resultado = await _imagenService.EliminarAsync(id);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = "Imagen eliminada permanentemente." });
    }
}