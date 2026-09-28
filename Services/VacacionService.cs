using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Helpers;
using Intranet.Services.Documentos;

namespace Intranet.Services;

public class VacacionService : IVacacionService
{
    private readonly AppDbContext _context;

    public VacacionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<VacacionDto>>> ObtenerTodasLasVacacionesAsync()
    {
        var movimientos = await _context.Vacaciones
            .Include(v => v.Usuario)
            .Include(v => v.RegistradoPor)
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
                RegistradoPorNombre = $"{v.RegistradoPor.Nombre} {v.RegistradoPor.Apellido}"
            })
            .ToListAsync();

        return ServiceResult<List<VacacionDto>>.Ok(movimientos);
    }

    public async Task<ServiceResult<SaldoVacacionesDto>> ObtenerSaldoAsync(long idUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<SaldoVacacionesDto>.Fallo("Usuario no encontrado.");

        if (!usuario.TieneVacaciones)
            return ServiceResult<SaldoVacacionesDto>.Fallo("Este usuario no tiene el beneficio de vacaciones habilitado.");

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

        if (!usuario.TieneVacaciones)
            return ServiceResult<VacacionDto>.Fallo("Este usuario no tiene el beneficio de vacaciones habilitado.");

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
            FechaInicio = DateTime.SpecifyKind(dto.FechaInicio, DateTimeKind.Utc),
            FechaFin = DateTime.SpecifyKind(dto.FechaFin, DateTimeKind.Utc),
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

        if (!usuario.TieneVacaciones)
            return ServiceResult<VacacionDto>.Fallo("Este usuario no tiene el beneficio de vacaciones habilitado.");

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

    // Resumen por empleado para las pantallas de RRHH (Gestión de Vacaciones / Saldos Personal).
    // Se incluyen TODOS los usuarios activos, incluso los que no tienen el beneficio,
    // para que el frontend los pueda mostrar deshabilitados en vez de ocultarlos sin explicación.
    public async Task<ServiceResult<List<ResumenVacacionesDto>>> ObtenerResumenAsync()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Cargo!)
                .ThenInclude(c => c.Area)
            .Where(u => u.Estado)
            .ToListAsync();

        var movimientos = await _context.Vacaciones
            .Where(v => v.Estado)
            .ToListAsync();

        var resumen = usuarios.Select(u =>
        {
            var movsUsuario = movimientos.Where(v => v.IdUsuario == u.IdUsuario).ToList();
            int diasDescontados = movsUsuario.Where(v => v.TipoMovimiento == "Descuento").Sum(v => v.DiasTomados);
            int diasAjustados = movsUsuario.Where(v => v.TipoMovimiento == "Ajuste").Sum(v => v.DiasTomados);

            return new ResumenVacacionesDto
            {
                IdUsuario = u.IdUsuario,
                Nombre = $"{u.Nombre} {u.Apellido}",
                Departamento = u.Cargo?.Area?.Nombre,
                FechaIngreso = u.FechaIngreso,
                TieneVacaciones = u.TieneVacaciones,
                DiasGanados = u.TieneVacaciones ? u.DiasVacacionesAsignados : 0,
                DiasTomados = u.TieneVacaciones ? diasDescontados : 0,
                SaldoDisponible = u.TieneVacaciones
                    ? (u.DiasVacacionesAsignados - diasDescontados + diasAjustados)
                    : 0
            };
        })
        .OrderBy(r => r.Nombre)
        .ToList();

        return ServiceResult<List<ResumenVacacionesDto>>.Ok(resumen);
    }

    // Anula un movimiento (Descuento o Ajuste) sin borrarlo de la BD.
    // Al ser un Descuento, el saldo se recalcula automáticamente porque
    // ObtenerSaldoAsync solo suma movimientos con Estado = true.
    public async Task<ServiceResult<bool>> AnularAsync(long idVacacion, long idAnuladoPor)
    {
        var vacacion = await _context.Vacaciones.FindAsync(idVacacion);
        if (vacacion == null)
            return ServiceResult<bool>.Fallo("El movimiento de vacaciones no existe.");

        if (!vacacion.Estado)
            return ServiceResult<bool>.Fallo("Este movimiento ya se encuentra anulado.");

        var anuladoPor = await _context.Usuarios.FindAsync(idAnuladoPor);

        vacacion.Estado = false;
        vacacion.Observacion = string.IsNullOrWhiteSpace(vacacion.Observacion)
            ? $"[Anulado por {anuladoPor?.Nombre} {anuladoPor?.Apellido} el {DateTime.UtcNow:dd/MM/yyyy}]"
            : $"{vacacion.Observacion} [Anulado por {anuladoPor?.Nombre} {anuladoPor?.Apellido} el {DateTime.UtcNow:dd/MM/yyyy}]";

        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
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

    // ============================================================
    // Solicitudes de vacaciones: empleado -> jefe directo -> RRHH
    // ============================================================

    public async Task<ServiceResult<SolicitudVacacionDto>> CrearSolicitudAsync(long idUsuario, SolicitudVacacionCrearDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo("Usuario no encontrado.");

        if (!usuario.TieneVacaciones)
            return ServiceResult<SolicitudVacacionDto>.Fallo("No tienes el beneficio de vacaciones habilitado.");

        if (usuario.IdJefeDirecto == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                "No tienes un jefe directo asignado. Pide a RRHH que lo configure antes de solicitar vacaciones.");

        if (dto.FechaFin.Date < dto.FechaInicio.Date)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La fecha de fin no puede ser anterior a la fecha de inicio.");

        if (dto.FechaInicio.Date < DateTime.UtcNow.Date)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La fecha de inicio no puede ser una fecha pasada.");

        // Días calendario, inclusivo (mismo criterio que el descuento directo de RRHH)
        int diasSolicitados = (dto.FechaFin.Date - dto.FechaInicio.Date).Days + 1;

        var saldoResult = await ObtenerSaldoAsync(idUsuario);
        if (!saldoResult.Exito)
            return ServiceResult<SolicitudVacacionDto>.Fallo(saldoResult.Mensaje!);

        if (diasSolicitados > saldoResult.Data!.DiasDisponibles)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                $"No tienes días suficientes. Disponibles: {saldoResult.Data.DiasDisponibles}, solicitados: {diasSolicitados}.");

        // Evita duplicar una solicitud sobre un rango que ya está en trámite o aprobado.
        bool yaExisteEnRango = await _context.SolicitudesVacaciones.AnyAsync(s =>
            s.IdUsuario == idUsuario &&
            s.Estado != EstadoSolicitudVacacion.RechazadaJefe &&
            s.Estado != EstadoSolicitudVacacion.RechazadaRrhh &&
            s.FechaInicio.Date <= dto.FechaFin.Date &&
            s.FechaFin.Date >= dto.FechaInicio.Date);

        if (yaExisteEnRango)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                "Ya tienes una solicitud en trámite o aprobada que se cruza con ese rango de fechas.");

        var solicitud = new SolicitudVacacion
        {
            IdUsuario = idUsuario,
            FechaInicio = DateTime.SpecifyKind(dto.FechaInicio.Date, DateTimeKind.Utc),
            FechaFin = DateTime.SpecifyKind(dto.FechaFin.Date, DateTimeKind.Utc),
            DiasSolicitados = diasSolicitados,
            Motivo = dto.Motivo,
            Estado = EstadoSolicitudVacacion.PendienteJefe,
            IdJefeAprobador = usuario.IdJefeDirecto,
            FechaSolicitud = DateTime.UtcNow
        };

        _context.SolicitudesVacaciones.Add(solicitud);
        await _context.SaveChangesAsync();

        return ServiceResult<SolicitudVacacionDto>.Ok(await MapearSolicitudDtoAsync(solicitud.IdSolicitud));
    }

    public async Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerMisSolicitudesAsync(long idUsuario)
    {
        var solicitudes = await ConsultaSolicitudesConIncludes()
            .Where(s => s.IdUsuario == idUsuario)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return ServiceResult<List<SolicitudVacacionDto>>.Ok(solicitudes.Select(MapearSolicitudDto).ToList());
    }

    public async Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerPendientesParaJefeAsync(long idJefe)
    {
        var solicitudes = await ConsultaSolicitudesConIncludes()
            .Where(s => s.IdJefeAprobador == idJefe && s.Estado == EstadoSolicitudVacacion.PendienteJefe)
            .OrderBy(s => s.FechaSolicitud)
            .ToListAsync();

        return ServiceResult<List<SolicitudVacacionDto>>.Ok(solicitudes.Select(MapearSolicitudDto).ToList());
    }

    public async Task<ServiceResult<SolicitudVacacionDto>> ResponderComoJefeAsync(long idSolicitud, long idJefe, RespuestaSolicitudVacacionDto dto)
    {
        var solicitud = await _context.SolicitudesVacaciones.FindAsync(idSolicitud);
        if (solicitud == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La solicitud no existe.");

        if (solicitud.IdJefeAprobador != idJefe)
            return ServiceResult<SolicitudVacacionDto>.Fallo("No tienes permiso para responder esta solicitud.");

        if (solicitud.Estado != EstadoSolicitudVacacion.PendienteJefe)
            return ServiceResult<SolicitudVacacionDto>.Fallo("Esta solicitud ya fue respondida y no puede modificarse.");

        solicitud.Estado = dto.Aprobar ? EstadoSolicitudVacacion.PendienteRrhh : EstadoSolicitudVacacion.RechazadaJefe;
        solicitud.FechaRespuestaJefe = DateTime.UtcNow;
        solicitud.ObservacionJefe = dto.Observacion;

        await _context.SaveChangesAsync();

        return ServiceResult<SolicitudVacacionDto>.Ok(await MapearSolicitudDtoAsync(solicitud.IdSolicitud));
    }

    public async Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerPendientesParaRrhhAsync()
    {
        var solicitudes = await ConsultaSolicitudesConIncludes()
            .Where(s => s.Estado == EstadoSolicitudVacacion.PendienteRrhh)
            .OrderBy(s => s.FechaSolicitud)
            .ToListAsync();

        return ServiceResult<List<SolicitudVacacionDto>>.Ok(solicitudes.Select(MapearSolicitudDto).ToList());
    }

    public async Task<ServiceResult<SolicitudVacacionDto>> ResponderComoRrhhAsync(long idSolicitud, long idRrhh, RespuestaSolicitudVacacionDto dto)
    {
        var solicitud = await _context.SolicitudesVacaciones.FindAsync(idSolicitud);
        if (solicitud == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La solicitud no existe.");

        if (solicitud.Estado != EstadoSolicitudVacacion.PendienteRrhh)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                "Esta solicitud no está pendiente de aprobación de RRHH (aún no la aprueba el jefe, o ya fue resuelta).");

        if (dto.Aprobar)
        {
            // Se vuelve a validar el saldo al momento de la aprobación final, por si
            // hubo movimientos (ajustes, otras solicitudes) desde que se creó la solicitud.
            var saldoResult = await ObtenerSaldoAsync(solicitud.IdUsuario);
            if (!saldoResult.Exito)
                return ServiceResult<SolicitudVacacionDto>.Fallo(saldoResult.Mensaje!);

            if (solicitud.DiasSolicitados > saldoResult.Data!.DiasDisponibles)
                return ServiceResult<SolicitudVacacionDto>.Fallo(
                    $"El empleado ya no tiene saldo suficiente. Disponibles: {saldoResult.Data.DiasDisponibles}, solicitados: {solicitud.DiasSolicitados}.");

            var vacacion = new Vacacion
            {
                IdUsuario = solicitud.IdUsuario,
                IdRegistradoPor = idRrhh,
                TipoMovimiento = "Descuento",
                FechaInicio = solicitud.FechaInicio,
                FechaFin = solicitud.FechaFin,
                DiasTomados = solicitud.DiasSolicitados,
                Observacion = $"Solicitud de vacaciones aprobada. Motivo: {solicitud.Motivo}",
                Estado = true
            };
            _context.Vacaciones.Add(vacacion);
            await _context.SaveChangesAsync(); // para obtener el Id generado

            solicitud.IdVacacionGenerada = vacacion.IdVacacion;
            solicitud.Estado = EstadoSolicitudVacacion.Aprobada;
        }
        else
        {
            solicitud.Estado = EstadoSolicitudVacacion.RechazadaRrhh;
        }

        solicitud.IdRrhhAprobador = idRrhh;
        solicitud.FechaRespuestaRrhh = DateTime.UtcNow;
        solicitud.ObservacionRrhh = dto.Observacion;

        await _context.SaveChangesAsync();

        return ServiceResult<SolicitudVacacionDto>.Ok(await MapearSolicitudDtoAsync(solicitud.IdSolicitud));
    }

    // Genera (sin guardar en ningún lado) la constancia en PDF de una solicitud
    // ya aprobada. Se arma en memoria en cada llamada: nada se persiste ni en
    // disco ni en base de datos, así que siempre refleja el estado más reciente.
    public async Task<ServiceResult<byte[]>> GenerarConstanciaAsync(long idSolicitud, long idUsuarioQueConsulta, bool esRrhh)
    {
        var solicitud = await _context.SolicitudesVacaciones
            .Include(s => s.Usuario)
                .ThenInclude(u => u.Cargo)
            .Include(s => s.JefeAprobador)
            .Include(s => s.RrhhAprobador)
            .FirstOrDefaultAsync(s => s.IdSolicitud == idSolicitud);

        if (solicitud == null)
            return ServiceResult<byte[]>.Fallo("La solicitud no existe.");

        if (!esRrhh && solicitud.IdUsuario != idUsuarioQueConsulta)
            return ServiceResult<byte[]>.Fallo("No tienes permiso para ver esta solicitud.");

        if (solicitud.Estado != EstadoSolicitudVacacion.Aprobada)
            return ServiceResult<byte[]>.Fallo(
                "Solo se puede generar la constancia de una solicitud ya aprobada por completo (jefe y RRHH).");

        // Informativo: el saldo actual del usuario para mostrarlo en el documento.
        var saldoResult = await ObtenerSaldoAsync(solicitud.IdUsuario);

        var pdf = ConstanciaVacacionesPdf.Generar(solicitud, saldoResult.Exito ? saldoResult.Data : null);
        return ServiceResult<byte[]>.Ok(pdf);
    }

    private IQueryable<SolicitudVacacion> ConsultaSolicitudesConIncludes()
        => _context.SolicitudesVacaciones
            .Include(s => s.Usuario)
            .Include(s => s.JefeAprobador)
            .Include(s => s.RrhhAprobador);

    private async Task<SolicitudVacacionDto> MapearSolicitudDtoAsync(long idSolicitud)
    {
        var s = await ConsultaSolicitudesConIncludes().FirstAsync(x => x.IdSolicitud == idSolicitud);
        return MapearSolicitudDto(s);
    }

    private static SolicitudVacacionDto MapearSolicitudDto(SolicitudVacacion s) => new()
    {
        IdSolicitud = s.IdSolicitud,
        IdUsuario = s.IdUsuario,
        SolicitanteNombre = $"{s.Usuario.Nombre} {s.Usuario.Apellido}",
        FechaInicio = s.FechaInicio,
        FechaFin = s.FechaFin,
        DiasSolicitados = s.DiasSolicitados,
        Motivo = s.Motivo,
        Estado = s.Estado,
        JefeAprobadorNombre = s.JefeAprobador != null ? $"{s.JefeAprobador.Nombre} {s.JefeAprobador.Apellido}" : null,
        FechaRespuestaJefe = s.FechaRespuestaJefe,
        ObservacionJefe = s.ObservacionJefe,
        RrhhAprobadorNombre = s.RrhhAprobador != null ? $"{s.RrhhAprobador.Nombre} {s.RrhhAprobador.Apellido}" : null,
        FechaRespuestaRrhh = s.FechaRespuestaRrhh,
        ObservacionRrhh = s.ObservacionRrhh,
        FechaSolicitud = s.FechaSolicitud
    };
}
