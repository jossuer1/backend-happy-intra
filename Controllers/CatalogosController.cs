using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Intranet.Helpers;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
// Nota: ya no hay [AllowAnonymous] a nivel de clase. En ASP.NET Core, si
// [AllowAnonymous] está en la clase, GANA sobre cualquier [Authorize] puesto
// en una acción individual (bypassa la autorización para TODO el controller).
// Por eso cada GET (lectura pública de catálogos) lleva su propio
// [AllowAnonymous], y los POST/PUT/DELETE que deben quedar restringidos
// (como los de Tipos de Sangre, exclusivos de ADMIN) NO lo llevan.
public class CatalogosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CatalogosController(AppDbContext context)
    {
        _context = context;
    }

    // --- OPCIONES FIJAS ---
    // Listas que se validan en el backend sin tener tabla propia (ver Models/OpcionesFijas.cs).
    // El frontend las usa para armar sus combos.
    [AllowAnonymous]
    [HttpGet("opciones-fijas")]
    public IActionResult GetOpcionesFijas()
        => Ok(new
        {
            tiposContrato = TiposContrato.Todos,
            tiposContratoConFechaFin = TiposContrato.ConFechaFin,
            jornadas = Jornadas.Todas,
            parentescosFamiliar = Parentescos.Todos
        });

    // --- ÁREAS ---
    [AllowAnonymous]
    [HttpGet("areas")]
    public async Task<IActionResult> GetAreas()
        => Ok(await _context.Areas.ToListAsync());

    [HttpPost("areas")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateArea([FromBody] CatalogoNombreDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Areas.AnyAsync(a => a.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe un área con ese nombre." });

        var area = new Area { Nombre = nombre };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();
        return Ok(area);
    }

    // --- CARGOS ---
    [AllowAnonymous]
    [HttpGet("cargos")]
    public async Task<IActionResult> GetCargos()
        => Ok(await _context.Cargos.ToListAsync());

    [HttpPost("cargos")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateCargo([FromBody] CargoCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (!await _context.Areas.AnyAsync(a => a.IdArea == dto.IdArea))
            return BadRequest(new { mensaje = "El área seleccionada no existe." });

        if (await _context.Cargos.AnyAsync(c => c.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe un cargo con ese nombre." });

        var cargo = new Cargo { Nombre = nombre, IdArea = dto.IdArea };
        _context.Cargos.Add(cargo);
        await _context.SaveChangesAsync();
        return Ok(cargo);
    }

    // --- BANCOS ---
    [AllowAnonymous]
    [HttpGet("bancos")]
    public async Task<IActionResult> GetBancos()
        => Ok(await _context.Bancos.ToListAsync());

    [HttpPost("bancos")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateBanco([FromBody] CatalogoNombreDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Bancos.AnyAsync(b => b.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe un banco con ese nombre." });

        var banco = new Banco { Nombre = nombre };
        _context.Bancos.Add(banco);
        await _context.SaveChangesAsync();
        return Ok(banco);
    }

    // --- REGIONES (solo lectura, catálogo fijo del Ecuador) ---
    [AllowAnonymous]
    [HttpGet("regiones")]
    public async Task<IActionResult> GetRegiones()
        => Ok(await _context.Regiones.Where(r => r.Estado).ToListAsync());

    // --- PROVINCIAS (solo lectura, catálogo fijo del Ecuador) ---
    [AllowAnonymous]
    [HttpGet("provincias")]
    public async Task<IActionResult> GetProvincias()
        => Ok(await _context.Provincias.Where(p => p.Estado).ToListAsync());

    // --- CIUDADES ---
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateCiudad([FromBody] CiudadCrearDto dto)
    {
        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Ciudades.AnyAsync(c => c.Nombre == nombre && c.IdProvincia == dto.IdProvincia))
            return BadRequest(new { mensaje = "Ya existe una ciudad con ese nombre en la provincia indicada." });

        var ciudad = new Ciudad { Nombre = nombre, IdProvincia = dto.IdProvincia };
        _context.Ciudades.Add(ciudad);
        await _context.SaveChangesAsync();
        return Ok(ciudad);
    }

    [HttpPut("ciudades/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateCiudad(long id, [FromBody] CiudadActualizarDto dto)
    {
        var ciudad = await _context.Ciudades.FindAsync(id);
        if (ciudad == null)
            return NotFound(new { mensaje = "Ciudad no encontrada." });

        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe." });

        ciudad.Nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);
        ciudad.IdProvincia = dto.IdProvincia;
        ciudad.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(ciudad);
    }

    [HttpDelete("ciudades/{id}")]
    [Authorize(Roles = "ADMIN")]
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
    [AllowAnonymous]
    [HttpGet("etnias")]
    public async Task<IActionResult> GetEtnias()
        => Ok(await _context.Etnias.Where(e => e.Estado).ToListAsync());

    [AllowAnonymous]
    [HttpGet("etnias/{id}")]
    public async Task<IActionResult> GetEtniaPorId(long id)
    {
        var etnia = await _context.Etnias.FindAsync(id);
        if (etnia == null)
            return NotFound(new { mensaje = "Etnia no encontrada." });
        return Ok(etnia);
    }

    [HttpPost("etnias")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEtnia([FromBody] EtniaCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Etnias.AnyAsync(e => e.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe una etnia con ese nombre." });

        var etnia = new Etnia { Nombre = nombre };
        _context.Etnias.Add(etnia);
        await _context.SaveChangesAsync();
        return Ok(etnia);
    }
    // --- ESTADOS CIVILES ---
    [AllowAnonymous]
    [HttpGet("estados-civiles")]
    public async Task<IActionResult> GetEstadosCiviles()
        => Ok(await _context.EstadosCiviles.Where(e => e.Estado).ToListAsync());

    [AllowAnonymous]
    [HttpGet("estados-civiles/{id}")]
    public async Task<IActionResult> GetEstadoCivilPorId(long id)
    {
        var estadoCivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadoCivil == null)
            return NotFound(new { mensaje = "Estado civil no encontrado." });

        return Ok(estadoCivil);
    }

    [HttpPost("estados-civiles")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateEstadoCivil([FromBody] EstadoCivilCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.EstadosCiviles.AnyAsync(e => e.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe un estado civil con ese nombre." });

        var estadoCivil = new EstadoCivil { Nombre = nombre };
        _context.EstadosCiviles.Add(estadoCivil);
        await _context.SaveChangesAsync();
        return Ok(estadoCivil);
    }

    [HttpPut("estados-civiles/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateEstadoCivil(long id, [FromBody] EstadoCivilActualizarDto dto)
    {
        var estadoCivil = await _context.EstadosCiviles.FindAsync(id);
        if (estadoCivil == null)
            return NotFound(new { mensaje = "Estado civil no encontrado." });

        estadoCivil.Nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);
        estadoCivil.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        return Ok(estadoCivil);
    }

    [HttpDelete("estados-civiles/{id}")]
    [Authorize(Roles = "ADMIN")]
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
    [AllowAnonymous]
    [HttpGet("generos")]
    public async Task<IActionResult> GetGeneros()
        => Ok(await _context.Generos.Where(g => g.Estado).ToListAsync());

    [AllowAnonymous]
    [HttpGet("generos/{id}")]
    public async Task<IActionResult> GetGeneroPorId(long id)
    {
        var genero = await _context.Generos.FindAsync(id);
        if (genero == null)
            return NotFound(new { mensaje = "Género no encontrado." });
        return Ok(genero);
    }

    [HttpPost("generos")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateGenero([FromBody] GeneroCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Generos.AnyAsync(g => g.Nombre == nombre))
            return BadRequest(new { mensaje = "Ya existe un género con ese nombre." });

        var genero = new Genero { Nombre = nombre };
        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();
        return Ok(genero);
    }

    // --- TIPOS DE SANGRE ---
    // Lectura pública (para el formulario de perfil); altas/edición/baja
    // exclusivas del rol ADMIN.
    [AllowAnonymous]
    [HttpGet("tipos-sangre")]
    public async Task<IActionResult> GetTiposSangre()
        => Ok(await _context.TiposSangre
            .Where(t => t.Estado)
            .Select(t => new TipoSangreReadDto { IdTipoSangre = t.IdTipoSangre, Nombre = t.Nombre, Estado = t.Estado })
            .ToListAsync());

    [AllowAnonymous]
    [HttpGet("tipos-sangre/{id}")]
    public async Task<IActionResult> GetTipoSangrePorId(long id)
    {
        var tipoSangre = await _context.TiposSangre.FindAsync(id);
        if (tipoSangre == null)
            return NotFound(new { mensaje = "Tipo de sangre no encontrado." });

        return Ok(new TipoSangreReadDto { IdTipoSangre = tipoSangre.IdTipoSangre, Nombre = tipoSangre.Nombre, Estado = tipoSangre.Estado });
    }

    [HttpPost("tipos-sangre")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateTipoSangre([FromBody] TipoSangreCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.TiposSangre.AnyAsync(t => t.Nombre == nombre))
            return BadRequest(new { mensaje = "Ese tipo de sangre ya existe en el catálogo." });

        var tipoSangre = new TipoSangre { Nombre = nombre };
        _context.TiposSangre.Add(tipoSangre);
        await _context.SaveChangesAsync();
        return Ok(new TipoSangreReadDto { IdTipoSangre = tipoSangre.IdTipoSangre, Nombre = tipoSangre.Nombre, Estado = tipoSangre.Estado });
    }

    [HttpPut("tipos-sangre/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateTipoSangre(long id, [FromBody] TipoSangreActualizarDto dto)
    {
        var tipoSangre = await _context.TiposSangre.FindAsync(id);
        if (tipoSangre == null)
            return NotFound(new { mensaje = "Tipo de sangre no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.TiposSangre.AnyAsync(t => t.Nombre == nombre && t.IdTipoSangre != id))
            return BadRequest(new { mensaje = "Ya existe otro tipo de sangre con ese nombre." });

        tipoSangre.Nombre = nombre;
        tipoSangre.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(new TipoSangreReadDto { IdTipoSangre = tipoSangre.IdTipoSangre, Nombre = tipoSangre.Nombre, Estado = tipoSangre.Estado });
    }

    [HttpDelete("tipos-sangre/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteTipoSangre(long id)
    {
        var tipoSangre = await _context.TiposSangre.FindAsync(id);
        if (tipoSangre == null)
            return NotFound(new { mensaje = "Tipo de sangre no encontrado." });

        tipoSangre.Estado = false; // baja lógica: no se borra físicamente para no romper usuarios ya asociados
        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Tipo de sangre desactivado correctamente." });
    }

}