using System.IO.Compression;
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
    // Solicitudes de vacaciones: el empleado solicita y su jefe directo aprueba o
    // rechaza. Si aprueba, se descuentan los días, se generan los 3 documentos y se
    // notifica por correo al empleado y a RRHH (RRHH no aprueba: solo recibe los firmados).
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

    // 10.1 Exclusivo RRHH: todas las solicitudes de la empresa, para ver su estado
    // y descargar los documentos de las aprobadas.
    [HttpGet("solicitudes/todas")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerTodasLasSolicitudes()
    {
        var resultado = await _vacacionService.ObtenerTodasLasSolicitudesAsync();
        return Ok(resultado.Data);
    }

    // 11. El jefe directo aprueba o rechaza la solicitud de un subordinado.
    // Si aprueba: descuento de días + documentos + correos. Si rechaza, el flujo termina ahí.
    [HttpPatch("solicitudes/{id}/jefe")]
    public async Task<IActionResult> ResponderComoJefe(long id, [FromBody] RespuestaSolicitudVacacionDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _vacacionService.ResponderComoJefeAsync(id, currentUserId, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // 12. Descarga los 3 documentos (solicitud, acta y declaración) de una solicitud
    // ya aprobada, en un ZIP. Se generan al vuelo; no se guardan en ningún lado.
    // Los puede descargar el empleado, su jefe directo y RRHH.
    [HttpGet("solicitudes/{id}/documentos")]
    public async Task<IActionResult> DescargarDocumentos(long id)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        var resultado = await _vacacionService.GenerarDocumentosAsync(id, currentUserId, esRrhh);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        using var memoria = new MemoryStream();
        using (var zip = new ZipArchive(memoria, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var doc in resultado.Data!)
            {
                var entrada = zip.CreateEntry(doc.NombreArchivo, CompressionLevel.Fastest);
                using var salida = entrada.Open();
                await salida.WriteAsync(doc.Contenido);
            }
        }

        return File(memoria.ToArray(), "application/zip", $"documentos-vacaciones-{id}.zip");
    }

    // 13. Descarga uno solo de los documentos: 1 = solicitud, 2 = acta, 3 = declaración.
    [HttpGet("solicitudes/{id}/documentos/{numero}")]
    public async Task<IActionResult> DescargarDocumento(long id, int numero)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        var resultado = await _vacacionService.GenerarDocumentosAsync(id, currentUserId, esRrhh);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        if (numero < 1 || numero > resultado.Data!.Count)
            return BadRequest(new { mensaje = "El número de documento debe ser 1 (solicitud), 2 (acta) o 3 (declaración)." });

        var documento = resultado.Data[numero - 1];
        return File(documento.Contenido, "application/pdf", documento.NombreArchivo);
    }
}
