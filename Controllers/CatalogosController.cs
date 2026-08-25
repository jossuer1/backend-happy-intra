using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
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

    // --- REGIONES (solo lectura, catálogo fijo del Ecuador) ---
    [HttpGet("regiones")]
    public async Task<IActionResult> GetRegiones()
        => Ok(await _context.Regiones.Where(r => r.Estado).ToListAsync());

    // --- PROVINCIAS (solo lectura, catálogo fijo del Ecuador) ---
    [HttpGet("provincias")]
    public async Task<IActionResult> GetProvincias()
        => Ok(await _context.Provincias.Where(p => p.Estado).ToListAsync());

    // --- CIUDADES ---
    [HttpGet("ciudades")]
    public async Task<ActionResult<IEnumerable<CiudadReadDto>>> GetCiudades()
    {
        var ciudades = await _context.Ciudades
            .Select(c => new CiudadReadDto
            {
                IdCiudad = c.IdCiudad,
                Nombre = c.Nombre,
                IdProvincia = c.IdProvincia,
                Estado = c.Estado,
                Provincia = c.Provincia != null ? new ProvinciaSimpleDto
                {
                    IdProvincia = c.Provincia.IdProvincia,
                    Nombre = c.Provincia.Nombre,
                    IdRegion = c.Provincia.IdRegion
                } : null
            })
            .ToListAsync();

        return Ok(ciudades);
    }
    [HttpGet("ciudades/{id}")]
    public async Task<IActionResult> GetCiudadPorId(long id)
    {
        var ciudad = await _context.Ciudades
            .Include(c => c.Provincia)
            .FirstOrDefaultAsync(c => c.IdCiudad == id);

        if (ciudad == null)
            return NotFound(new { mensaje = "Ciudad no encontrada." });

        return Ok(ciudad);
    }

    [HttpPost("ciudades")]
    public async Task<IActionResult> CreateCiudad([FromBody] CiudadCrearDto dto)
    {
        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe." });

        var ciudad = new Ciudad { Nombre = dto.Nombre, IdProvincia = dto.IdProvincia };
        _context.Ciudades.Add(ciudad);
        await _context.SaveChangesAsync();
        return Ok(ciudad);
    }

    [HttpPut("ciudades/{id}")]
    public async Task<IActionResult> UpdateCiudad(long id, [FromBody] CiudadActualizarDto dto)
    {
        var ciudad = await _context.Ciudades.FindAsync(id);
        if (ciudad == null)
            return NotFound(new { mensaje = "Ciudad no encontrada." });

        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe." });

        ciudad.Nombre = dto.Nombre;
        ciudad.IdProvincia = dto.IdProvincia;
        ciudad.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(ciudad);
    }

    [HttpDelete("ciudades/{id}")]
    public async Task<IActionResult> DeleteCiudad(long id)
    {
        var ciudad = await _context.Ciudades.FindAsync(id);
        if (ciudad == null)
            return NotFound(new { mensaje = "Ciudad no encontrada." });

        ciudad.Estado = false; // baja lógica: no se elimina físicamente para no romper usuarios ya asociados
        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Ciudad desactivada correctamente." });
    }

    // --- ETNIAS ---
    [HttpGet("etnias")]
    public async Task<IActionResult> GetEtnias()
        => Ok(await _context.Etnias.Where(e => e.Estado).ToListAsync());

    [HttpGet("etnias/{id}")]
    public async Task<IActionResult> GetEtniaPorId(long id)
    {
        var etnia = await _context.Etnias.FindAsync(id);
        if (etnia == null)
            return NotFound(new { mensaje = "Etnia no encontrada." });
        return Ok(etnia);
    }

    [HttpPost("etnias")]
    public async Task<IActionResult> CreateEtnia([FromBody] EtniaCrearDto dto)
    {
        var etnia = new Etnia { Nombre = dto.Nombre };
        _context.Etnias.Add(etnia);
        await _context.SaveChangesAsync();
        return Ok(etnia);
    }
    // --- ESTADOS CIVILES ---
    [HttpGet("estados-civiles")]
    public async Task<IActionResult> GetEstadosCiviles()
        => Ok(await _context.EstadosCiviles.Where(e => e.Estado).ToListAsync());

    [HttpGet("estados-civiles/{id}")]
    public async Task<IActionResult> GetEstadoCivilPorId(long id)
    {
        var estadoCivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadoCivil == null)
            return NotFound(new { mensaje = "Estado civil no encontrado." });

        return Ok(estadoCivil);
    }

    [HttpPost("estados-civiles")]
    public async Task<IActionResult> CreateEstadoCivil([FromBody] EstadoCivilCrearDto dto)
    {
        var estadoCivil = new EstadoCivil { Nombre = dto.Nombre };
        _context.EstadosCiviles.Add(estadoCivil);
        await _context.SaveChangesAsync();
        return Ok(estadoCivil);
    }

    [HttpPut("estados-civiles/{id}")]
    public async Task<IActionResult> UpdateEstadoCivil(long id, [FromBody] EstadoCivilActualizarDto dto)
    {
        var estadoCivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadoCivil == null)
            return NotFound(new { mensaje = "Estado civil no encontrado." });

        estadoCivil.Nombre = dto.Nombre;
        estadoCivil.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        return Ok(estadoCivil);
    }

    [HttpDelete("estados-civiles/{id}")]
    public async Task<IActionResult> DeleteEstadoCivil(long id)
    {
        var estadoCivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadoCivil == null)
            return NotFound(new { mensaje = "Estado civil no encontrado." });

        estadoCivil.Estado = false; // Baja lógica
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Estado civil desactivado correctamente." });
    }


    // --- GENEROS ---
    [HttpGet("generos")]
    public async Task<IActionResult> GetGeneros()
        => Ok(await _context.Generos.Where(g => g.Estado).ToListAsync());

    [HttpGet("generos/{id}")]
    public async Task<IActionResult> GetGeneroPorId(long id)
    {
        var genero = await _context.Generos.FindAsync(id);
        if (genero == null)
            return NotFound(new { mensaje = "Género no encontrado." });
        return Ok(genero);
    }

    [HttpPost("generos")]
    public async Task<IActionResult> CreateGenero([FromBody] GeneroCrearDto dto)
    {
        var genero = new Genero { Nombre = dto.Nombre };
        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();
        return Ok(genero);
    }

}