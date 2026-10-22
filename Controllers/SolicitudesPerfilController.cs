using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Intranet.DTOs;
using Intranet.Services;

namespace Intranet.Controllers;

// Solicitudes de cambio de perfil: el empleado propone (PUT /api/Usuarios/mi-perfil),
// RRHH revisa el comparativo y aprueba o rechaza.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudesPerfilController : ControllerBase
{
    private readonly ISolicitudPerfilService _service;

    public SolicitudesPerfilController(ISolicitudPerfilService service)
    {
        _service = service;
    }

    private long UsuarioActualId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // El empleado ve el historial de sus propias solicitudes (y los motivos de rechazo).
    [HttpGet("mias")]
    public async Task<IActionResult> Mias()
    {
        var resultado = await _service.ObtenerMiasAsync(UsuarioActualId);
        return Ok(resultado.Data);
    }

    // RRHH: listado general. ?estado=PENDIENTE | APROBADA | RECHAZADA (opcional).
    [HttpGet]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Listar([FromQuery] string? estado)
    {
        var resultado = await _service.ListarAsync(estado);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Detalle con comparativo antes/después. RRHH ve cualquiera; un empleado, solo las suyas.
    [HttpGet("{id}")]
    public async Task<IActionResult> Detalle(long id)
    {
        var resultado = await _service.ObtenerDetalleAsync(id, UsuarioActualId, User.IsInRole("RRHH"));

        if (!resultado.Exito)
            return NotFound(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    [HttpPost("{id}/aprobar")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Aprobar(long id, [FromBody] ResolverSolicitudCambioPerfilDto? dto)
    {
        var resultado = await _service.AprobarAsync(id, UsuarioActualId, dto?.Observacion);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = "Cambios aprobados y aplicados al perfil del empleado." });
    }

    [HttpPost("{id}/rechazar")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Rechazar(long id, [FromBody] ResolverSolicitudCambioPerfilDto dto)
    {
        var resultado = await _service.RechazarAsync(id, UsuarioActualId, dto.Observacion ?? string.Empty);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = "Solicitud rechazada. Se le devolvió la edición al empleado." });
    }
}
