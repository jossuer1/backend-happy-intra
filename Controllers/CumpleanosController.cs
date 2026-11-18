using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Intranet.DTOs;
using Intranet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers;

[ApiController]
[Route("api/cumpleanos")]
[Authorize(Roles = "RRHH")]
public class CumpleanosController(CumpleanosService service) : ControllerBase
{
    [HttpGet("proximos")]
    public Task<IReadOnlyList<CumpleanosProximoDto>> ObtenerProximos(
        CancellationToken ct, [FromQuery, Range(0, 366, ErrorMessage = "El rango debe ser de 0 a 366 días.")] int dias = 30)
        => service.ObtenerProximosAsync(dias, ct);

    [HttpPost("{idUsuario:long}/felicitar")]
    public async Task<IActionResult> Felicitar(long idUsuario, CancellationToken ct)
    {
        var idRrhh = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await service.FelicitarAsync(idUsuario, idRrhh, ct);
        return resultado.Exito
            ? Ok(new { mensaje = "Felicitación enviada." })
            : BadRequest(new { mensaje = resultado.Mensaje });
    }
}
