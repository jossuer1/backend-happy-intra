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
}