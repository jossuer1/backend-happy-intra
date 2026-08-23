using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;

namespace Intranet.Services;

public class VacacionService : IVacacionService
{
    private readonly AppDbContext _context;

    public VacacionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<SaldoVacacionesDto>> ObtenerSaldoAsync(long idUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<SaldoVacacionesDto>.Fallo("Usuario no encontrado.");

        var movimientos = await _context.Vacaciones
            .Where(v => v.IdUsuario == idUsuario && v.Estado)
            .ToListAsync();

        int diasDescontados = movimientos.Where(v => v.TipoMovimiento == "Descuento").Sum(v => v.DiasTomados);
        int diasAjustados = movimientos.Where(v => v.TipoMovimiento == "Ajuste").Sum(v => v.DiasTomados);

        var saldo = new SaldoVacacionesDto
        {
            IdUsuario = idUsuario,
            DiasAsignados = usuario.DiasVacacionesAsignados,
            DiasDescontados = diasDescontados,
            DiasAjustados = diasAjustados,
            DiasDisponibles = usuario.DiasVacacionesAsignados - diasDescontados + diasAjustados
        };

        return ServiceResult<SaldoVacacionesDto>.Ok(saldo);
    }

    public async Task<ServiceResult<VacacionDto>> RegistrarDescuentoAsync(VacacionDescuentoCrearDto dto, long idRegistradoPor)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return ServiceResult<VacacionDto>.Fallo("Usuario no encontrado.");

        if (dto.FechaFin.Date < dto.FechaInicio.Date)
            return ServiceResult<VacacionDto>.Fallo("La fecha de fin no puede ser anterior a la fecha de inicio.");

        // Días calendario, inclusivo (ej. lunes a viernes de la misma semana = 5 días)
        int diasSolicitados = (dto.FechaFin.Date - dto.FechaInicio.Date).Days + 1;

        var saldoResult = await ObtenerSaldoAsync(dto.IdUsuario);
        if (!saldoResult.Exito)
            return ServiceResult<VacacionDto>.Fallo(saldoResult.Mensaje!);

        var saldo = saldoResult.Data!;
        if (diasSolicitados > saldo.DiasDisponibles)
            return ServiceResult<VacacionDto>.Fallo(
                $"El usuario no tiene días suficientes. Disponibles: {saldo.DiasDisponibles}, solicitados: {diasSolicitados}.");

        var vacacion = new Models.Vacacion
        {
            IdUsuario = dto.IdUsuario,
            IdRegistradoPor = idRegistradoPor,
            TipoMovimiento = "Descuento",
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            DiasTomados = diasSolicitados,
            Observacion = dto.Motivo,
            Estado = true
        };

        _context.Vacaciones.Add(vacacion);
        await _context.SaveChangesAsync();

        return ServiceResult<VacacionDto>.Ok(await MapearDtoAsync(vacacion.IdVacacion));
    }

    public async Task<ServiceResult<VacacionDto>> RegistrarAjusteAsync(VacacionAjusteCrearDto dto, long idRegistradoPor)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return ServiceResult<VacacionDto>.Fallo("Usuario no encontrado.");

        if (dto.Dias <= 0)
            return ServiceResult<VacacionDto>.Fallo("El número de días a corregir debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(dto.Motivo))
            return ServiceResult<VacacionDto>.Fallo("El motivo del ajuste es obligatorio.");

        var vacacion = new Models.Vacacion
        {
            IdUsuario = dto.IdUsuario,
            IdRegistradoPor = idRegistradoPor,
            TipoMovimiento = "Ajuste",
            FechaInicio = null,
            FechaFin = null,
            DiasTomados = dto.Dias,
            Observacion = dto.Motivo,
            Estado = true
        };

        _context.Vacaciones.Add(vacacion);
        await _context.SaveChangesAsync();

        return ServiceResult<VacacionDto>.Ok(await MapearDtoAsync(vacacion.IdVacacion));
    }

    public async Task<ServiceResult<List<VacacionDto>>> ObtenerHistorialAsync(long idUsuario)
    {
        var movimientos = await _context.Vacaciones
            .Include(v => v.RegistradoPor)
            .Where(v => v.IdUsuario == idUsuario)
            .OrderByDescending(v => v.FechaRegistro)
            .Select(v => new VacacionDto
            {
                IdVacacion = v.IdVacacion,
                TipoMovimiento = v.TipoMovimiento,
                FechaInicio = v.FechaInicio,
                FechaFin = v.FechaFin,
                DiasTomados = v.DiasTomados,
                Observacion = v.Observacion,
                FechaRegistro = v.FechaRegistro,
                RegistradoPorNombre = v.RegistradoPor.Nombre + " " + v.RegistradoPor.Apellido
            })
            .ToListAsync();

        return ServiceResult<List<VacacionDto>>.Ok(movimientos);
    }

    private async Task<VacacionDto> MapearDtoAsync(long idVacacion)
    {
        var v = await _context.Vacaciones
            .Include(x => x.RegistradoPor)
            .FirstAsync(x => x.IdVacacion == idVacacion);

        return new VacacionDto
        {
            IdVacacion = v.IdVacacion,
            TipoMovimiento = v.TipoMovimiento,
            FechaInicio = v.FechaInicio,
            FechaFin = v.FechaFin,
            DiasTomados = v.DiasTomados,
            Observacion = v.Observacion,
            FechaRegistro = v.FechaRegistro,
            RegistradoPorNombre = $"{v.RegistradoPor.Nombre} {v.RegistradoPor.Apellido}"
        };
    }
}
