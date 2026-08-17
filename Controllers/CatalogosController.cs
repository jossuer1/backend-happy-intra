using Intranet.Data;
using Intranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CatalogosController(AppDbContext context)
    {
        _context = context;
    }

    // --- ÁREAS ---
    [HttpGet("areas")]
    public async Task<IActionResult> GetAreas()
        => Ok(await _context.Areas.ToListAsync());

    [HttpPost("areas")]
    public async Task<IActionResult> CreateArea([FromBody] string nombre)
    {
        var area = new Area { Nombre = nombre };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();
        return Ok(area);
    }

    // --- CARGOS ---
    [HttpGet("cargos")]
    public async Task<IActionResult> GetCargos()
        => Ok(await _context.Cargos.ToListAsync());

    [HttpPost("cargos")]
    public async Task<IActionResult> CreateCargo([FromBody] string nombre)
    {
        var cargo = new Cargo { Nombre = nombre };
        _context.Cargos.Add(cargo);
        await _context.SaveChangesAsync();
        return Ok(cargo);
    }

    // --- BANCOS ---
    [HttpGet("bancos")]
    public async Task<IActionResult> GetBancos()
        => Ok(await _context.Bancos.ToListAsync());

    [HttpPost("bancos")]
    public async Task<IActionResult> CreateBanco([FromBody] string nombre)
    {
        var banco = new Banco { Nombre = nombre };
        _context.Bancos.Add(banco);
        await _context.SaveChangesAsync();
        return Ok(banco);
    }
}