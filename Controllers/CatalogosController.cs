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
            parentescosFamiliar = Parentescos.Todos,
            nacionalidades = Nacionalidades.Todas
        });

    // Los GET públicos solo devuelven registros activos (para los combos). Un ADMIN
    // puede pedir ?incluirInactivos=true para verlos todos en su pantalla de gestión
    // (el token se lee aunque el endpoint sea [AllowAnonymous]).
    private bool VerInactivos(bool incluirInactivos)
        => incluirInactivos && User.IsInRole("ADMIN");

    // --- ÁREAS ---
    [AllowAnonymous]
    [HttpGet("areas")]
    public async Task<IActionResult> GetAreas([FromQuery] bool incluirInactivos = false)
    {
        var query = _context.Areas.AsNoTracking().AsQueryable();
        if (!VerInactivos(incluirInactivos))
            query = query.Where(a => a.Estado);

        return Ok(await query.OrderBy(a => a.Nombre).ToListAsync());
    }

    [HttpPost("areas")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateArea([FromBody] CatalogoNombreDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Areas.AnyAsync(a => a.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe un área con ese nombre." });

        var area = new Area { Nombre = nombre };
        _context.Areas.Add(area);
        await _context.SaveChangesAsync();
        return Ok(area);
    }

    [HttpPut("areas/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateArea(long id, [FromBody] CatalogoActualizarDto dto)
    {
        var area = await _context.Areas.FindAsync(id);
        if (area == null)
            return NotFound(new { mensaje = "Área no encontrada." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Areas.AnyAsync(a => a.Nombre.ToUpper() == nombre && a.IdArea != id))
            return BadRequest(new { mensaje = "Ya existe otra área con ese nombre." });

        // Orden del catálogo: no se puede desactivar un área que aún tiene cargos activos.
        if (!dto.Estado && area.Estado &&
            await _context.Cargos.AnyAsync(c => c.IdArea == id && c.Estado))
            return BadRequest(new { mensaje = "No se puede desactivar el área: todavía tiene cargos activos." });

        area.Nombre = nombre;
        area.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(area);
    }

    // --- CARGOS ---
    [AllowAnonymous]
    [HttpGet("cargos")]
    public async Task<IActionResult> GetCargos([FromQuery] long? idArea = null, [FromQuery] bool incluirInactivos = false)
    {
        var query = _context.Cargos.AsNoTracking().AsQueryable();
        if (idArea.HasValue)
            query = query.Where(c => c.IdArea == idArea.Value);
        if (!VerInactivos(incluirInactivos))
            query = query.Where(c => c.Estado);

        return Ok(await query.OrderBy(c => c.Nombre).ToListAsync());
    }

    [HttpPost("cargos")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateCargo([FromBody] CargoCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (!await _context.Areas.AnyAsync(a => a.IdArea == dto.IdArea))
            return BadRequest(new { mensaje = "El área seleccionada no existe." });

        if (await _context.Cargos.AnyAsync(c => c.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe un cargo con ese nombre." });

        var cargo = new Cargo { Nombre = nombre, IdArea = dto.IdArea };
        _context.Cargos.Add(cargo);
        await _context.SaveChangesAsync();
        return Ok(cargo);
    }

    [HttpPut("cargos/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateCargo(long id, [FromBody] CargoActualizarDto dto)
    {
        var cargo = await _context.Cargos.FindAsync(id);
        if (cargo == null)
            return NotFound(new { mensaje = "Cargo no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (!await _context.Areas.AnyAsync(a => a.IdArea == dto.IdArea && a.Estado))
            return BadRequest(new { mensaje = "El área seleccionada no existe o está inactiva." });

        if (await _context.Cargos.AnyAsync(c => c.Nombre.ToUpper() == nombre && c.IdCargo != id))
            return BadRequest(new { mensaje = "Ya existe otro cargo con ese nombre." });

        cargo.Nombre = nombre;
        cargo.IdArea = dto.IdArea;
        cargo.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(cargo);
    }

    // --- BANCOS ---
    [AllowAnonymous]
    [HttpGet("bancos")]
    public async Task<IActionResult> GetBancos([FromQuery] bool incluirInactivos = false)
    {
        var query = _context.Bancos.AsNoTracking().AsQueryable();
        if (!VerInactivos(incluirInactivos))
            query = query.Where(b => b.Estado);

        return Ok(await query.OrderBy(b => b.Nombre).ToListAsync());
    }

    [HttpPost("bancos")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateBanco([FromBody] CatalogoNombreDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Bancos.AnyAsync(b => b.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe un banco con ese nombre." });

        var banco = new Banco { Nombre = nombre };
        _context.Bancos.Add(banco);
        await _context.SaveChangesAsync();
        return Ok(banco);
    }

    [HttpPut("bancos/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateBanco(long id, [FromBody] CatalogoActualizarDto dto)
    {
        var banco = await _context.Bancos.FindAsync(id);
        if (banco == null)
            return NotFound(new { mensaje = "Banco no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Bancos.AnyAsync(b => b.Nombre.ToUpper() == nombre && b.IdBanco != id))
            return BadRequest(new { mensaje = "Ya existe otro banco con ese nombre." });

        banco.Nombre = nombre;
        banco.Estado = dto.Estado;
        await _context.SaveChangesAsync();
        return Ok(banco);
    }

    // --- REGIONES (solo lectura, catálogo fijo del Ecuador) ---
    [AllowAnonymous]
    [HttpGet("regiones")]
    public async Task<IActionResult> GetRegiones()
        => Ok(await _context.Regiones.Where(r => r.Estado).ToListAsync());

    // --- PROVINCIAS ---
    // Jerarquía: Región → Provincia → Ciudad. Cada provincia pertenece a una región.
    // Lectura pública (con filtro opcional ?idRegion=); alta/edición solo ADMIN.
    [AllowAnonymous]
    [HttpGet("provincias")]
    public async Task<IActionResult> GetProvincias([FromQuery] long? idRegion = null, [FromQuery] bool incluirInactivos = false)
    {
        var query = _context.Provincias.AsNoTracking().AsQueryable();
        if (idRegion.HasValue)
            query = query.Where(p => p.IdRegion == idRegion.Value);
        if (!VerInactivos(incluirInactivos))
            query = query.Where(p => p.Estado);

        var provincias = await query
            .OrderBy(p => p.Nombre)
            .Select(p => new ProvinciaReadDto
            {
                IdProvincia = p.IdProvincia,
                Nombre = p.Nombre,
                IdRegion = p.IdRegion,
                NombreRegion = p.Region.Nombre,
                Estado = p.Estado
            })
            .ToListAsync();

        return Ok(provincias);
    }

    [HttpPost("provincias")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateProvincia([FromBody] ProvinciaCrearDto dto)
    {
        if (!await _context.Regiones.AnyAsync(r => r.IdRegion == dto.IdRegion && r.Estado))
            return BadRequest(new { mensaje = "La región indicada no existe o está inactiva." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Provincias.AnyAsync(p => p.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe una provincia con ese nombre." });

        var provincia = new Provincia { Nombre = nombre, IdRegion = dto.IdRegion };
        _context.Provincias.Add(provincia);
        await _context.SaveChangesAsync();

        return Ok(new ProvinciaReadDto
        {
            IdProvincia = provincia.IdProvincia,
            Nombre = provincia.Nombre,
            IdRegion = provincia.IdRegion,
            Estado = provincia.Estado
        });
    }

    [HttpPut("provincias/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateProvincia(long id, [FromBody] ProvinciaActualizarDto dto)
    {
        var provincia = await _context.Provincias.FindAsync(id);
        if (provincia == null)
            return NotFound(new { mensaje = "Provincia no encontrada." });

        if (!await _context.Regiones.AnyAsync(r => r.IdRegion == dto.IdRegion && r.Estado))
            return BadRequest(new { mensaje = "La región indicada no existe o está inactiva." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Provincias.AnyAsync(p => p.Nombre.ToUpper() == nombre && p.IdProvincia != id))
            return BadRequest(new { mensaje = "Ya existe otra provincia con ese nombre." });

        // Orden del catálogo: no se puede desactivar una provincia con ciudades activas.
        if (!dto.Estado && provincia.Estado &&
            await _context.Ciudades.AnyAsync(c => c.IdProvincia == id && c.Estado))
            return BadRequest(new { mensaje = "No se puede desactivar la provincia: todavía tiene ciudades activas." });

        provincia.Nombre = nombre;
        provincia.IdRegion = dto.IdRegion;
        provincia.Estado = dto.Estado;
        await _context.SaveChangesAsync();

        return Ok(new ProvinciaReadDto
        {
            IdProvincia = provincia.IdProvincia,
            Nombre = provincia.Nombre,
            IdRegion = provincia.IdRegion,
            Estado = provincia.Estado
        });
    }

    // --- CIUDADES ---
    // Cada ciudad pertenece a una provincia (y esta a una región). Filtro opcional ?idProvincia=
    [AllowAnonymous]
    [HttpGet("ciudades")]
    public async Task<ActionResult<IEnumerable<CiudadReadDto>>> GetCiudades(
        [FromQuery] long? idProvincia = null, [FromQuery] bool incluirInactivos = false)
    {
        var query = _context.Ciudades.AsNoTracking().AsQueryable();
        if (idProvincia.HasValue)
            query = query.Where(c => c.IdProvincia == idProvincia.Value);
        if (!VerInactivos(incluirInactivos))
            query = query.Where(c => c.Estado);

        var ciudades = await query
            .OrderBy(c => c.Nombre)
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
                    IdRegion = c.Provincia.IdRegion,
                    NombreRegion = c.Provincia.Region.Nombre
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
            .AsNoTracking()
            .Where(c => c.IdCiudad == id)
            .Select(c => new CiudadReadDto
            {
                IdCiudad = c.IdCiudad,
                Nombre = c.Nombre,
                IdProvincia = c.IdProvincia,
                Estado = c.Estado,
                Provincia = new ProvinciaSimpleDto
                {
                    IdProvincia = c.Provincia.IdProvincia,
                    Nombre = c.Provincia.Nombre,
                    IdRegion = c.Provincia.IdRegion,
                    NombreRegion = c.Provincia.Region.Nombre
                }
            })
            .FirstOrDefaultAsync();

        if (ciudad == null)
            return NotFound(new { mensaje = "Ciudad no encontrada." });

        return Ok(ciudad);
    }

    [HttpPost("ciudades")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateCiudad([FromBody] CiudadCrearDto dto)
    {
        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia && p.Estado);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe o está inactiva." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Ciudades.AnyAsync(c => c.Nombre.ToUpper() == nombre && c.IdProvincia == dto.IdProvincia))
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

        var provinciaExiste = await _context.Provincias.AnyAsync(p => p.IdProvincia == dto.IdProvincia && p.Estado);
        if (!provinciaExiste)
            return BadRequest(new { mensaje = "La provincia indicada no existe o está inactiva." });

        var nombreCiudad = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Ciudades.AnyAsync(c => c.Nombre.ToUpper() == nombreCiudad && c.IdProvincia == dto.IdProvincia && c.IdCiudad != id))
            return BadRequest(new { mensaje = "Ya existe una ciudad con ese nombre en la provincia indicada." });

        ciudad.Nombre = nombreCiudad;
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

        if (await _context.Etnias.AnyAsync(e => e.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe una etnia con ese nombre." });

        var etnia = new Etnia { Nombre = nombre };
        _context.Etnias.Add(etnia);
        await _context.SaveChangesAsync();
        return Ok(etnia);
    }

    [HttpPut("etnias/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateEtnia(long id, [FromBody] EtniaActualizarDto dto)
    {
        var etnia = await _context.Etnias.FindAsync(id);
        if (etnia == null)
            return NotFound(new { mensaje = "Etnia no encontrada." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Etnias.AnyAsync(e => e.Nombre.ToUpper() == nombre && e.IdEtnia != id))
            return BadRequest(new { mensaje = "Ya existe otra etnia con ese nombre." });

        etnia.Nombre = nombre;
        etnia.Estado = dto.Estado;
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

        if (await _context.EstadosCiviles.AnyAsync(e => e.Nombre.ToUpper() == nombre))
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

        if (await _context.Generos.AnyAsync(g => g.Nombre.ToUpper() == nombre))
            return BadRequest(new { mensaje = "Ya existe un género con ese nombre." });

        var genero = new Genero { Nombre = nombre };
        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();
        return Ok(genero);
    }

    [HttpPut("generos/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateGenero(long id, [FromBody] GeneroActualizarDto dto)
    {
        var genero = await _context.Generos.FindAsync(id);
        if (genero == null)
            return NotFound(new { mensaje = "Género no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Generos.AnyAsync(g => g.Nombre.ToUpper() == nombre && g.IdGenero != id))
            return BadRequest(new { mensaje = "Ya existe otro género con ese nombre." });

        genero.Nombre = nombre;
        genero.Estado = dto.Estado;
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

        if (await _context.TiposSangre.AnyAsync(t => t.Nombre.ToUpper() == nombre))
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

        if (await _context.TiposSangre.AnyAsync(t => t.Nombre.ToUpper() == nombre && t.IdTipoSangre != id))
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

    // --- FORMULARIO DE PERFIL: todos los catálogos en una sola petición ---
    [AllowAnonymous]
    [HttpGet("formulario-perfil")]
    public async Task<IActionResult> GetCatalogosFormularioPerfil()
    {
        var areas = await _context.Areas.AsNoTracking().Where(a => a.Estado).ToListAsync();
        var cargos = await _context.Cargos.AsNoTracking().Where(c => c.Estado).ToListAsync();
        var bancos = await _context.Bancos.AsNoTracking().Where(b => b.Estado).ToListAsync();

        var ciudades = await _context.Ciudades
            .AsNoTracking()
            .Where(c => c.Estado)
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
                    IdRegion = c.Provincia.IdRegion,
                    NombreRegion = c.Provincia.Region.Nombre
                } : null
            })
            .ToListAsync();

        var etnias = await _context.Etnias.AsNoTracking().Where(e => e.Estado).ToListAsync();
        var estadosCiviles = await _context.EstadosCiviles.AsNoTracking().Where(e => e.Estado).ToListAsync();
        var generos = await _context.Generos.AsNoTracking().Where(g => g.Estado).ToListAsync();

        var tiposSangre = await _context.TiposSangre
            .AsNoTracking()
            .Where(t => t.Estado)
            .Select(t => new TipoSangreReadDto
            {
                IdTipoSangre = t.IdTipoSangre,
                Nombre = t.Nombre,
                Estado = t.Estado
            })
            .ToListAsync();

        return Ok(new
        {
            areas,
            cargos,
            bancos,
            ciudades,
            etnias,
            estadosCiviles,
            generos,
            tiposSangre,
            // No consulta la BD: son las listas fijas de Models/OpcionesFijas.cs
            opcionesFijas = new
            {
                tiposContrato = TiposContrato.Todos,
                tiposContratoConFechaFin = TiposContrato.ConFechaFin,
                jornadas = Jornadas.Todas,
                parentescosFamiliar = Parentescos.Todos,
                nacionalidades = Nacionalidades.Todas
            }
        });
    }

}