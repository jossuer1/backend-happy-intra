using Intranet.Helpers;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public partial class UsuarioService
{
    private async Task<string?> ValidarSaldoFinalAsync(Usuario u)
    {
        if (u.DiasVacacionesAsignados is < 0 or > 365) return "Los días asignados deben estar entre 0 y 365.";
        if (!u.TieneVacaciones) return null;
        var movimientos = await _context.Vacaciones.AsNoTracking().Where(v => v.IdUsuario == u.IdUsuario && v.Estado)
            .Select(v => new { v.TipoMovimiento, v.DiasTomados }).ToListAsync();
        var saldo = u.DiasVacacionesAsignados + movimientos.Where(v => v.TipoMovimiento == "Ajuste").Sum(v => v.DiasTomados)
            - movimientos.Where(v => v.TipoMovimiento == "Descuento").Sum(v => v.DiasTomados);
        return saldo < 0 ? "No puedes reducir los días asignados porque el saldo resultante sería negativo." : null;
    }

    private async Task<string?> ValidarCatalogosAsync(Usuario u)
    {
        if (!await _context.Cargos.AnyAsync(c => c.IdCargo == u.IdCargo && c.Estado && c.Area.Estado))
            return "El cargo y su área deben existir y estar activos.";
        if (u.IdCiudad.HasValue && !await _context.Ciudades.AnyAsync(c => c.IdCiudad == u.IdCiudad && c.Estado && c.Provincia.Estado && c.Provincia.Region.Estado))
            return "La ciudad, provincia y región deben existir y estar activas.";
        if (u.IdGenero.HasValue && !await _context.Generos.AnyAsync(c => c.IdGenero == u.IdGenero && c.Estado)) return "El género no existe o está inactivo.";
        if (u.IdEtnia.HasValue && !await _context.Etnias.AnyAsync(c => c.IdEtnia == u.IdEtnia && c.Estado)) return "La etnia no existe o está inactiva.";
        if (u.IdEstadoCivil.HasValue && !await _context.EstadosCiviles.AnyAsync(c => c.IdEstadoCivil == u.IdEstadoCivil && c.Estado)) return "El estado civil no existe o está inactivo.";
        var bancos = u.DatosBancarios.Where(b => b.Estado).Select(b => b.IdBanco).Distinct().ToArray();
        if (await _context.Bancos.CountAsync(b => bancos.Contains(b.IdBanco) && b.Estado) != bancos.Length)
            return "Todos los bancos seleccionados deben existir y estar activos.";
        return null;
    }

    private async Task<bool> FormaCicloAsync(long usuarioId, long jefeId)
    {
        var jerarquia = await _context.Usuarios.AsNoTracking().Select(u => new { u.IdUsuario, u.IdJefeDirecto }).ToDictionaryAsync(u => u.IdUsuario, u => u.IdJefeDirecto);
        var visitados = new HashSet<long> { usuarioId };
        long? actual = jefeId;
        while (actual.HasValue)
        {
            if (!visitados.Add(actual.Value)) return true;
            if (!jerarquia.TryGetValue(actual.Value, out actual)) break;
        }
        return false;
    }
}
