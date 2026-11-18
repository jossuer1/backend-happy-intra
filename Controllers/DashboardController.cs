using Intranet.DTOs;
using Intranet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "RRHH")]
public class DashboardController(DashboardRrhhService service) : ControllerBase
{
    [HttpGet("rrhh")]
    public Task<ResumenRrhhDto> ObtenerResumen(CancellationToken ct) => service.ObtenerResumenAsync(ct);
}
