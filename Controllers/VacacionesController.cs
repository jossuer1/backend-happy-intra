using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Intranet.DTOs;
using Intranet.Services;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere estar autenticado
public class VacacionesController : ControllerBase
{
    private readonly IVacacionService _vacacionService;

    public VacacionesController(IVacacionService vacacionService)
    {
        _vacacionService = vacacionService;
    }

    // 1. Obtener el saldo propio o el de cualquier usuario si es RRHH
    [HttpGet("saldo/{idUsuario}")]
    public async Task<IActionResult> ObtenerSaldo(long idUsuario)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        if (!esRrhh && currentUserId != idUsuario)
            return Forbid(); // Un empleado normal no puede ver el saldo de otros

        var resultado = await _vacacionService.ObtenerSaldoAsync(idUsuario);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 2. Obtener historial propio o el de cualquier usuario si es RRHH
    [HttpGet("historial/{idUsuario}")]
    public async Task<IActionResult> ObtenerHistorial(long idUsuario)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        if (!esRrhh && currentUserId != idUsuario)
            return Forbid();

        var resultado = await _vacacionService.ObtenerHistorialAsync(idUsuario);
        return Ok(resultado.Data);
    }

    // 3. Endpoint conveniente para que el empleado logueado vea su propio historial directamente
    [HttpGet("mis-vacaciones")]
    public async Task<IActionResult> ObtenerMisVacaciones()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ObtenerHistorialAsync(currentUserId);
        return Ok(resultado.Data);
    }

    // 4. Exclusivo RRHH: Obtener el registro global de vacaciones de toda la empresa
    [HttpGet("todas")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerTodas()
    {
        var resultado = await _vacacionService.ObtenerTodasLasVacacionesAsync();
        return Ok(resultado.Data);
    }

    // 4.1 Exclusivo RRHH: Resumen por empleado (saldo actual) para las pantallas
    // de "Gestión de Vacaciones" y "Saldos Personal". Incluye a quienes NO tienen
    // el beneficio habilitado, marcados con tieneVacaciones = false.
    [HttpGet("resumen")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerResumen()
    {
        var resultado = await _vacacionService.ObtenerResumenAsync();
        return Ok(resultado.Data);
    }

    // 5. Registrar descuento de vacaciones (RRHH)
    [HttpPost("descuento")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> RegistrarDescuento([FromBody] VacacionDescuentoCrearDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.RegistrarDescuentoAsync(dto, currentUserId);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 6. Registrar ajuste/corrección de vacaciones (RRHH)
    [HttpPost("ajuste")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> RegistrarAjuste([FromBody] VacacionAjusteCrearDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.RegistrarAjusteAsync(dto, currentUserId);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 7. Exclusivo RRHH: Anular un movimiento (Descuento o Ajuste) registrado por error.
    // No borra el registro físicamente, lo marca Estado = false para conservar
    // el historial/auditoría, y el saldo se recalcula automáticamente.
    [HttpDelete("{id}")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Anular(long id)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.AnularAsync(id, currentUserId);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new { mensaje = "Movimiento de vacaciones anulado correctamente." });
    }

    // ============================================================
    // Solicitudes de vacaciones: el empleado solicita, primero responde
    // su jefe directo y, si aprueba, RRHH da el visto bueno final.
    // ============================================================

    // 8. El empleado logueado crea una solicitud de vacaciones.
    [HttpPost("solicitudes")]
    public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudVacacionCrearDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.CrearSolicitudAsync(currentUserId, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 9. El empleado logueado ve el estado de todas sus propias solicitudes.
    [HttpGet("solicitudes/mis-solicitudes")]
    public async Task<IActionResult> ObtenerMisSolicitudes()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ObtenerMisSolicitudesAsync(currentUserId);
        return Ok(resultado.Data);
    }

    // 10. Cualquier usuario que sea jefe directo de alguien ve las solicitudes
    // de sus subordinados que están esperando su respuesta. No requiere un rol
    // especial: "ser jefe" es una relación (Usuario.IdJefeDirecto), no un rol.
    [HttpGet("solicitudes/pendientes-jefe")]
    public async Task<IActionResult> ObtenerPendientesParaJefe()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ObtenerPendientesParaJefeAsync(currentUserId);
        return Ok(resultado.Data);
    }

    // 11. El jefe directo aprueba o rechaza la solicitud de un subordinado.
    // Si aprueba, la solicitud pasa a RRHH; si rechaza, el flujo termina ahí.
    [HttpPatch("solicitudes/{id}/jefe")]
    public async Task<IActionResult> ResponderComoJefe(long id, [FromBody] RespuestaSolicitudVacacionDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ResponderComoJefeAsync(id, currentUserId, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 12. Exclusivo RRHH: solicitudes que el jefe ya aprobó y esperan el visto
    // bueno final antes de descontar los días del saldo del empleado.
    [HttpGet("solicitudes/pendientes-rrhh")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerPendientesParaRrhh()
    {
        var resultado = await _vacacionService.ObtenerPendientesParaRrhhAsync();
        return Ok(resultado.Data);
    }

    // 13. Exclusivo RRHH: aprobación final. Si aprueba, se genera automáticamente
    // el movimiento de descuento y se debitan los días del saldo del empleado.
    [HttpPatch("solicitudes/{id}/rrhh")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ResponderComoRrhh(long id, [FromBody] RespuestaSolicitudVacacionDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ResponderComoRrhhAsync(id, currentUserId, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 14. Genera (al vuelo, sin guardarla en ningún lado) la constancia en PDF
    // de una solicitud ya aprobada. El propio empleado puede descargar la suya;
    // RRHH puede descargar la de cualquiera. Cada llamada reconstruye el PDF
    // desde cero con los datos más recientes.
    [HttpGet("solicitudes/{id}/constancia")]
    public async Task<IActionResult> GenerarConstancia(long id)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        var resultado = await _vacacionService.GenerarConstanciaAsync(id, currentUserId, esRrhh);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return File(resultado.Data!, "application/pdf", $"constancia-vacaciones-{id}.pdf");
    }
}