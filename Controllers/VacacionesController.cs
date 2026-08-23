using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Intranet.DTOs;
using Intranet.Services;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VacacionesController : ControllerBase
{
    private readonly IVacacionService _vacacionService;

    public VacacionesController(IVacacionService vacacionService)
    {
        _vacacionService = vacacionService;
    }

    // Solo RRHH puede descontar días de vacaciones a un usuario
    [HttpPost("descuento")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> RegistrarDescuento([FromBody] VacacionDescuentoCrearDto dto)
    {
        var idRegistradoPor = ObtenerIdUsuarioActual();
        var resultado = await _vacacionService.RegistrarDescuentoAsync(dto, idRegistradoPor);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Solo RRHH puede devolver días por corrección de un error
    [HttpPost("ajuste")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> RegistrarAjuste([FromBody] VacacionAjusteCrearDto dto)
    {
        var idRegistradoPor = ObtenerIdUsuarioActual();
        var resultado = await _vacacionService.RegistrarAjusteAsync(dto, idRegistradoPor);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Un usuario ve su propio saldo; RRHH puede ver el de cualquiera
    [HttpGet("usuario/{idUsuario}/saldo")]
    public async Task<IActionResult> GetSaldo(long idUsuario)
    {
        if (!PuedeVerDatosDe(idUsuario))
            return Forbid();

        var resultado = await _vacacionService.ObtenerSaldoAsync(idUsuario);
        if (!resultado.Exito)
            return NotFound(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Un usuario ve su propio historial; RRHH puede ver el de cualquiera
    [HttpGet("usuario/{idUsuario}")]
    public async Task<IActionResult> GetHistorial(long idUsuario)
    {
        if (!PuedeVerDatosDe(idUsuario))
            return Forbid();

        var resultado = await _vacacionService.ObtenerHistorialAsync(idUsuario);
        return Ok(resultado.Data);
    }

    private long ObtenerIdUsuarioActual()
        => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool PuedeVerDatosDe(long idUsuario)
        => User.IsInRole("RRHH") || ObtenerIdUsuarioActual() == idUsuario;
}
