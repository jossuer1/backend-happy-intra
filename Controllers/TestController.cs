using Intranet.Data;
using Intranet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public TestController(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet("conexion")]
    public async Task<IActionResult> TestConexion()
    {
        try
        {
            var rolesCount = await _context.Roles.CountAsync();
            var roles = await _context.Roles.ToListAsync();

            return Ok(new
            {
                mensaje = "Conexión a PostgreSQL en Clever Cloud exitosa",
                totalRoles = rolesCount,
                datos = roles
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                mensaje = "Error al conectar con la base de datos",
                error = ex.Message
            });
        }
    }

    [HttpPost("email")]
    public async Task<IActionResult> TestEmail([FromBody] EmailTestDto request)
    {
        var enviado = await _emailService.SendEmailAsync(
            request.Destinatario,
            request.Nombre,
            "Prueba de correo - Intranet API",
            "<h1>¡Hola!</h1><p>Esta es una prueba de envío exitosa usando <strong>Brevo API</strong> desde ASP.NET Core.</p>"
        );

        if (enviado)
            return Ok(new { mensaje = $"Correo enviado exitosamente a {request.Destinatario}" });

        return StatusCode(500, new { mensaje = "No se pudo enviar el correo. Revisa la API Key y el correo remitente en Brevo." });
    }
}

public class EmailTestDto
{
    public string Destinatario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}