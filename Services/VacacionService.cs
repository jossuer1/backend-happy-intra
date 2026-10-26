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
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<VacacionService> _logger;

    public VacacionService(AppDbContext context, IEmailService emailService, IConfiguration config, ILogger<VacacionService> logger)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    // Npgsql (timestamptz) solo acepta DateTime con Kind=Utc.
    // Como en vacaciones solo importa el DÍA, se normaliza a medianoche UTC.
    private static DateTime AFechaUtc(DateTime fecha)
        => DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);

    // "Hoy" según la hora de Ecuador (UTC-5), para no adelantar el día por la noche.
    private static DateTime ObtenerHoyEcuador()
    {
        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");
        }
        catch (TimeZoneNotFoundException)
        {
            // Windows sin soporte de IDs IANA
            tz = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        }
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;
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
        await using var tx = await _context.Database.BeginTransactionAsync();
        await BloquearTraspasoEquipoAsync();
        if (!ValidacionesExpediente.FechaInformada(dto.FechaInicio) || !ValidacionesExpediente.FechaInformada(dto.FechaFin))
            return ServiceResult<VacacionDto>.Fallo("Las fechas de inicio y fin son obligatorias y deben ser válidas.");
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return ServiceResult<VacacionDto>.Fallo("Usuario no encontrado.");

        if (!usuario.TieneVacaciones)
            return ServiceResult<VacacionDto>.Fallo("Este usuario no tiene el beneficio de vacaciones habilitado.");

        if (dto.FechaFin.Date < dto.FechaInicio.Date)
            return ServiceResult<VacacionDto>.Fallo("La fecha de fin no puede ser anterior a la fecha de inicio.");

        var inicio = AFechaUtc(dto.FechaInicio);
        var fin = AFechaUtc(dto.FechaFin);

        // Días calendario, inclusivo (ej. lunes a viernes de la misma semana = 5 días)
        int diasSolicitados = (fin - inicio).Days + 1;

        if (await _context.Vacaciones.AnyAsync(v => v.IdUsuario == dto.IdUsuario && v.Estado &&
            v.TipoMovimiento == "Descuento" && v.FechaInicio <= fin && v.FechaFin >= inicio) ||
            await _context.SolicitudesVacaciones.AnyAsync(s => s.IdUsuario == dto.IdUsuario &&
            s.Estado != EstadoSolicitudVacacion.RechazadaJefe && s.Estado != EstadoSolicitudVacacion.Anulada &&
            s.FechaInicio <= fin && s.FechaFin >= inicio))
            return ServiceResult<VacacionDto>.Fallo("El rango coincide con vacaciones o solicitudes vigentes del empleado.");

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
            FechaInicio = inicio,
            FechaFin = fin,
            DiasTomados = diasSolicitados,
            Observacion = dto.Motivo,
            Estado = true
        };
        _context.Vacaciones.Add(vacacion);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return ServiceResult<VacacionDto>.Ok(await MapearDtoAsync(vacacion.IdVacacion));
    }

    public async Task<ServiceResult<VacacionDto>> RegistrarAjusteAsync(VacacionAjusteCrearDto dto, long idRegistradoPor)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        await BloquearTraspasoEquipoAsync();
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return ServiceResult<VacacionDto>.Fallo("Usuario no encontrado.");

        if (!usuario.TieneVacaciones)
            return ServiceResult<VacacionDto>.Fallo("Este usuario no tiene el beneficio de vacaciones habilitado.");

        if (dto.Dias <= 0 || dto.Dias > 365)
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
        await tx.CommitAsync();

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
                RegistradoPorNombre = v.RegistradoPor.Nombre + " " + v.RegistradoPor.Apellido,
                IdSolicitud = _context.SolicitudesVacaciones
                    .Where(s => s.IdVacacionGenerada == v.IdVacacion)
                    .Select(s => (long?)s.IdSolicitud)
                    .FirstOrDefault()
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
        await using var tx = await _context.Database.BeginTransactionAsync();
        await BloquearTraspasoEquipoAsync();
        var vacacion = await _context.Vacaciones.FindAsync(idVacacion);
        if (vacacion == null)
            return ServiceResult<bool>.Fallo("El movimiento de vacaciones no existe.");

        if (!vacacion.Estado)
            return ServiceResult<bool>.Fallo("Este movimiento ya se encuentra anulado.");

        var anuladoPor = await _context.Usuarios.FindAsync(idAnuladoPor);

        if (vacacion.TipoMovimiento == "Ajuste")
        {
            var saldo = await ObtenerSaldoAsync(vacacion.IdUsuario);
            if (!saldo.Exito || saldo.Data!.DiasDisponibles < vacacion.DiasTomados)
                return ServiceResult<bool>.Fallo("No se puede anular el ajuste porque dejaría un saldo negativo.");
        }
        vacacion.Estado = false;
        vacacion.Observacion = string.IsNullOrWhiteSpace(vacacion.Observacion)
            ? $"[Anulado por {anuladoPor?.Nombre} {anuladoPor?.Apellido} el {DateTime.UtcNow:dd/MM/yyyy}]"
            : $"{vacacion.Observacion} [Anulado por {anuladoPor?.Nombre} {anuladoPor?.Apellido} el {DateTime.UtcNow:dd/MM/yyyy}]";

        // Si este descuento nació de una solicitud, esa solicitud queda ANULADA
        // (así no se generan más documentos y el rango de fechas se libera).
        var solicitudOrigen = await _context.SolicitudesVacaciones
            .FirstOrDefaultAsync(x => x.IdVacacionGenerada == idVacacion);
        if (solicitudOrigen != null && solicitudOrigen.Estado == EstadoSolicitudVacacion.Aprobada)
            solicitudOrigen.Estado = EstadoSolicitudVacacion.Anulada;

        await _context.SaveChangesAsync();

        await tx.CommitAsync();
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
        if (!ValidacionesExpediente.FechaInformada(dto.FechaInicio) || !ValidacionesExpediente.FechaInformada(dto.FechaFin))
            return ServiceResult<SolicitudVacacionDto>.Fallo("Las fechas de inicio y fin son obligatorias y deben ser válidas.");
        await using var tx = await _context.Database.BeginTransactionAsync();
        await BloquearTraspasoEquipoAsync();
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo("Usuario no encontrado.");

        if (!usuario.TieneVacaciones)
            return ServiceResult<SolicitudVacacionDto>.Fallo("No tienes el beneficio de vacaciones habilitado.");

        if (usuario.IdJefeDirecto == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                "No tienes un jefe directo asignado. Pide a RRHH que lo configure antes de solicitar vacaciones.");
        if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == usuario.IdJefeDirecto && u.Estado && u.EsJefe && u.IdUsuario != idUsuario))
            return ServiceResult<SolicitudVacacionDto>.Fallo("Tu jefe directo debe estar activo y habilitado para aprobar vacaciones.");

        if (dto.FechaFin.Date < dto.FechaInicio.Date)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La fecha de fin no puede ser anterior a la fecha de inicio.");

        // "Hoy" en Ecuador (UTC-5) para que después de las 19:00 no se adelante el día.
        if (dto.FechaInicio.Date < ObtenerHoyEcuador())
            return ServiceResult<SolicitudVacacionDto>.Fallo("La fecha de inicio no puede ser una fecha pasada.");

        // Fechas normalizadas a medianoche UTC (Kind=Utc): requisito de Npgsql para timestamptz.
        // Se usan tanto en la consulta como al guardar.
        var inicio = AFechaUtc(dto.FechaInicio);
        var fin = AFechaUtc(dto.FechaFin);

        // Días calendario, inclusivo (mismo criterio que el descuento directo de RRHH)
        int diasSolicitados = (fin - inicio).Days + 1;

        var saldoResult = await ObtenerSaldoAsync(idUsuario);
        if (!saldoResult.Exito)
            return ServiceResult<SolicitudVacacionDto>.Fallo(saldoResult.Mensaje!);

        if (diasSolicitados > saldoResult.Data!.DiasDisponibles)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                $"No tienes días suficientes. Disponibles: {saldoResult.Data.DiasDisponibles}, solicitados: {diasSolicitados}.");

        // Evita duplicar una solicitud sobre un rango que ya está en trámite o aprobado.
        // Se comparan las columnas directamente (sin .Date) con parámetros UTC.
        bool yaExisteEnRango = await _context.SolicitudesVacaciones.AnyAsync(s =>
            s.IdUsuario == idUsuario &&
            s.Estado != EstadoSolicitudVacacion.RechazadaJefe &&
            s.Estado != EstadoSolicitudVacacion.Anulada &&
            s.FechaInicio <= fin &&
            s.FechaFin >= inicio);

        if (yaExisteEnRango)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                "Ya tienes una solicitud en trámite o aprobada que se cruza con ese rango de fechas.");
        if (await _context.Vacaciones.AnyAsync(v => v.IdUsuario == idUsuario && v.Estado &&
            v.TipoMovimiento == "Descuento" && v.FechaInicio <= fin && v.FechaFin >= inicio))
            return ServiceResult<SolicitudVacacionDto>.Fallo("Ya tienes vacaciones registradas en ese rango de fechas.");

        var solicitud = new SolicitudVacacion
        {
            IdUsuario = idUsuario,
            FechaInicio = inicio,
            FechaFin = fin,
            DiasSolicitados = diasSolicitados,
            Motivo = dto.Motivo,
            Estado = EstadoSolicitudVacacion.PendienteJefe,
            IdJefeAprobador = usuario.IdJefeDirecto,
            FechaSolicitud = DateTime.UtcNow
        };

        _context.SolicitudesVacaciones.Add(solicitud);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        await EnviarAvisoSolicitudAsync(solicitud.IdSolicitud, nueva: true);

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

    // El jefe directo es la única aprobación del flujo. Si aprueba:
    //  1) se descuentan los días del saldo (movimiento "Descuento" en Vacaciones),
    //  2) se generan los 3 documentos y se envían por correo al empleado,
    //  3) se avisa a RRHH (sin adjuntos) para que tenga presente la entrega de los firmados.
    // Si el correo falla, la aprobación NO se revierte: los documentos siguen
    // disponibles para descargar desde la intranet.
    public async Task<ServiceResult<SolicitudVacacionDto>> ResponderComoJefeAsync(long idSolicitud, long idJefe, RespuestaSolicitudVacacionDto dto)
    {
        if (!dto.Aprobar.HasValue)
            return ServiceResult<SolicitudVacacionDto>.Fallo("Indica explícitamente si apruebas o rechazas la solicitud.");
        if (dto.Aprobar == false && string.IsNullOrWhiteSpace(dto.Observacion))
            return ServiceResult<SolicitudVacacionDto>.Fallo("El motivo del rechazo es obligatorio.");
        await using var tx = await _context.Database.BeginTransactionAsync();
        await BloquearTraspasoEquipoAsync();
        var solicitud = await _context.SolicitudesVacaciones.FindAsync(idSolicitud);
        if (solicitud == null)
            return ServiceResult<SolicitudVacacionDto>.Fallo("La solicitud no existe.");

        if (solicitud.IdJefeAprobador != idJefe)
            return ServiceResult<SolicitudVacacionDto>.Fallo("No tienes permiso para responder esta solicitud.");
        if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == idJefe && u.Estado && u.EsJefe))
            return ServiceResult<SolicitudVacacionDto>.Fallo("Tu cuenta debe estar activa y habilitada como jefe.");

        if (solicitud.Estado != EstadoSolicitudVacacion.PendienteJefe)
            return ServiceResult<SolicitudVacacionDto>.Fallo("Esta solicitud ya fue respondida y no puede modificarse.");

        solicitud.FechaRespuestaJefe = DateTime.UtcNow;
        solicitud.ObservacionJefe = dto.Observacion;

        if (dto.Aprobar == false)
        {
            solicitud.Estado = EstadoSolicitudVacacion.RechazadaJefe;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            await EnviarAvisoSolicitudAsync(solicitud.IdSolicitud, nueva: false);
            return ServiceResult<SolicitudVacacionDto>.Ok(await MapearSolicitudDtoAsync(solicitud.IdSolicitud));
        }

        // Se vuelve a validar el saldo al momento de aprobar, por si hubo movimientos
        // (ajustes, otras solicitudes) desde que se creó la solicitud.
        var saldoResult = await ObtenerSaldoAsync(solicitud.IdUsuario);
        if (!saldoResult.Exito)
            return ServiceResult<SolicitudVacacionDto>.Fallo(saldoResult.Mensaje!);

        if (solicitud.DiasSolicitados > saldoResult.Data!.DiasDisponibles)
            return ServiceResult<SolicitudVacacionDto>.Fallo(
                $"El empleado ya no tiene saldo suficiente. Disponibles: {saldoResult.Data.DiasDisponibles}, solicitados: {solicitud.DiasSolicitados}.");
        if (await _context.Vacaciones.AnyAsync(v => v.IdUsuario == solicitud.IdUsuario && v.Estado &&
            v.TipoMovimiento == "Descuento" && v.FechaInicio <= solicitud.FechaFin && v.FechaFin >= solicitud.FechaInicio))
            return ServiceResult<SolicitudVacacionDto>.Fallo("El rango de la solicitud ya tiene vacaciones registradas.");

        // Un solo SaveChanges: el descuento y la aprobación se guardan juntos o no se guarda nada.
        solicitud.VacacionGenerada = new Vacacion
        {
            IdUsuario = solicitud.IdUsuario,
            IdRegistradoPor = idJefe,
            TipoMovimiento = "Descuento",
            FechaInicio = solicitud.FechaInicio,
            FechaFin = solicitud.FechaFin,
            DiasTomados = solicitud.DiasSolicitados,
            Observacion = $"Solicitud de vacaciones aprobada por el jefe directo. Motivo: {solicitud.Motivo}",
            Estado = true
        };
        solicitud.Estado = EstadoSolicitudVacacion.Aprobada;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        await EnviarCorreosAprobacionAsync(solicitud.IdSolicitud);

        return ServiceResult<SolicitudVacacionDto>.Ok(await MapearSolicitudDtoAsync(solicitud.IdSolicitud));
    }

    // Mismo orden de bloqueo que el traspaso: una solicitud no puede nacer con
    // el jefe anterior ni ser resuelta por él mientras se está trasladando.
    private Task BloquearTraspasoEquipoAsync()
        => _context.Database.ExecuteSqlRawAsync(
            "LOCK TABLE \"Usuarios\", \"SolicitudesVacaciones\" IN SHARE ROW EXCLUSIVE MODE");

    // Arma los 3 PDFs de una solicitud aprobada. Se generan al vuelo en cada llamada;
    // no se guardan en disco ni en base de datos.
    // Pueden pedirlos el propio empleado, su jefe directo y RRHH.
    public async Task<ServiceResult<List<DocumentoPdf>>> GenerarDocumentosAsync(long idSolicitud, long idUsuarioQueConsulta, bool esRrhh)
    {
        var solicitud = await ConsultaParaDocumentos().FirstOrDefaultAsync(s => s.IdSolicitud == idSolicitud);

        if (solicitud == null)
            return ServiceResult<List<DocumentoPdf>>.Fallo("La solicitud no existe.");

        if (!esRrhh && solicitud.IdUsuario != idUsuarioQueConsulta && solicitud.IdJefeAprobador != idUsuarioQueConsulta)
            return ServiceResult<List<DocumentoPdf>>.Fallo("No tienes permiso para ver esta solicitud.");

        if (solicitud.Estado == EstadoSolicitudVacacion.Anulada)
            return ServiceResult<List<DocumentoPdf>>.Fallo("Estas vacaciones fueron anuladas; ya no se pueden generar los documentos.");

        if (solicitud.Estado != EstadoSolicitudVacacion.Aprobada)
            return ServiceResult<List<DocumentoPdf>>.Fallo("Los documentos solo se generan cuando el jefe directo ya aprobó la solicitud.");

        try
        {
            var datos = await ConstruirDatosDocumentosAsync(solicitud);
            return ServiceResult<List<DocumentoPdf>>.Ok(GenerarPdfs(datos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar los documentos de la solicitud {Id}.", idSolicitud);
            return ServiceResult<List<DocumentoPdf>>.Fallo("No se pudieron generar los documentos. Inténtalo de nuevo o contacta a soporte.");
        }
    }

    public async Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerTodasLasSolicitudesAsync()
    {
        var solicitudes = await ConsultaSolicitudesConIncludes()
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return ServiceResult<List<SolicitudVacacionDto>>.Ok(solicitudes.Select(MapearSolicitudDto).ToList());
    }

    private static List<DocumentoPdf> GenerarPdfs(DatosDocumentoVacaciones datos) => new()
    {
        new("1-solicitud-de-vacaciones.pdf", SolicitudVacacionesPdf.Generar(datos)),
        new("2-acta-de-vacaciones.pdf", ActaVacacionesPdf.Generar(datos)),
        new("3-declaracion-goce-y-pago-de-vacaciones.pdf", DeclaratoriaVacacionesPdf.Generar(datos))
    };

    private IQueryable<SolicitudVacacion> ConsultaParaDocumentos()
        => _context.SolicitudesVacaciones
            .Include(s => s.Usuario).ThenInclude(u => u.Cargo).ThenInclude(c => c!.Area)
            .Include(s => s.Usuario).ThenInclude(u => u.Ciudad)
            .Include(s => s.Usuario).ThenInclude(u => u.Genero)
            .Include(s => s.JefeAprobador)
            .Include(s => s.VacacionGenerada);

    // Calcula todos los datos de los documentos. El saldo se toma "tal como quedó al
    // aprobar esta solicitud" (movimientos hasta el descuento de esta solicitud), para
    // que si el documento se vuelve a descargar después no cambien los días pendientes.
    private async Task<DatosDocumentoVacaciones> ConstruirDatosDocumentosAsync(SolicitudVacacion s)
    {
        var u = s.Usuario;
        long idDescuento = s.IdVacacionGenerada ?? long.MaxValue;

        var movimientos = await _context.Vacaciones
            .Where(v => v.IdUsuario == s.IdUsuario && v.Estado && v.IdVacacion <= idDescuento)
            .ToListAsync();

        int descontados = movimientos.Where(v => v.TipoMovimiento == "Descuento").Sum(v => v.DiasTomados);
        int ajustados = movimientos.Where(v => v.TipoMovimiento == "Ajuste").Sum(v => v.DiasTomados);

        int pendientes = u.DiasVacacionesAsignados - descontados + ajustados;
        int disponiblesAntes = pendientes + s.DiasSolicitados;
        int consumidosAntes = descontados - ajustados - s.DiasSolicitados;

        var generoNombre = u.Genero?.Nombre ?? "";
        bool esMujer = generoNombre.StartsWith("F", StringComparison.OrdinalIgnoreCase)
                    || generoNombre.Contains("mujer", StringComparison.OrdinalIgnoreCase);

        var fechaEmision = CalculosVacaciones.AHoraEcuador(s.FechaRespuestaJefe ?? DateTime.UtcNow).Date;

        return new DatosDocumentoVacaciones
        {
            RazonSocial = _config["Empresa:RazonSocial"] ?? "HAPPYPAY S.A.",
            RepresentanteLegal = _config["Empresa:RepresentanteLegal"] ?? "EDWIN FABIAN VASQUEZ PIEDRA",

            NombreCompleto = $"{u.Nombre} {u.Apellido}",
            Cedula = u.Cedula,
            Area = u.Cargo?.Area?.Nombre ?? "",
            Cargo = u.Cargo?.Nombre ?? "",
            FechaIngreso = u.FechaIngreso,
            EsMujer = esMujer,
            Nacionalidad = u.Nacionalidad,

            NombreJefe = s.JefeAprobador != null ? $"{s.JefeAprobador.Nombre} {s.JefeAprobador.Apellido}" : "",

            // La ciudad es la del empleado; si no la tiene cargada se usa la ciudad por defecto.
            Ciudad = u.Ciudad?.Nombre ?? _config["Empresa:CiudadPorDefecto"] ?? "Quito",
            FechaEmision = fechaEmision,

            PeriodoLaboral = CalculosVacaciones.PeriodoLaboral(u.FechaIngreso, consumidosAntes),
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            FechaReingreso = CalculosVacaciones.FechaReingreso(s.FechaFin),

            DiasGozados = s.DiasSolicitados,
            DiasDisponiblesAntes = disponiblesAntes,
            DiasPendientes = pendientes,

            Observacion = s.Motivo,

            // El pago de las vacaciones va en el rol del mes en que EMPIEZAN.
            MesRol = CalculosVacaciones.NombreMes(s.FechaInicio.Month),
            AnioRol = s.FechaInicio.Year
        };
    }

    // Correo al empleado (con los 3 PDFs adjuntos) y aviso a RRHH (sin adjuntos).
    // Todo va dentro de try/catch: un fallo de correo nunca debe deshacer la aprobación.
    private async Task EnviarAvisoSolicitudAsync(long idSolicitud, bool nueva)
    {
        try
        {
            var solicitud = await ConsultaSolicitudesConIncludes().AsNoTracking()
                .FirstAsync(s => s.IdSolicitud == idSolicitud);
            var destinatario = nueva ? solicitud.JefeAprobador : solicitud.Usuario;
            if (destinatario == null)
            {
                _logger.LogWarning("Solicitud {Id}: no existe destinatario para el aviso de vacaciones.", idSolicitud);
                return;
            }
            var correo = string.IsNullOrWhiteSpace(destinatario.CorreoEmpresa)
                ? destinatario.CorreoPersonal : destinatario.CorreoEmpresa;
            var url = (_config["Intranet:UrlBase"] ?? "https://intranet-happy.onrender.com").TrimEnd('/')
                + (nueva ? "/aprobar-vacaciones" : "/mis-vacaciones");
            var html = PlantillasCorreo.AvisoSolicitudVacaciones(
                destinatario.Nombre, $"{solicitud.Usuario.Nombre} {solicitud.Usuario.Apellido}",
                solicitud.IdSolicitud, solicitud.FechaInicio, solicitud.FechaFin,
                solicitud.DiasSolicitados, nueva ? solicitud.Motivo : solicitud.ObservacionJefe, url, nueva);
            var enviado = await _emailService.SendEmailAsync(correo,
                $"{destinatario.Nombre} {destinatario.Apellido}",
                nueva ? "Nueva solicitud de vacaciones de tu equipo" : "Tu solicitud de vacaciones fue rechazada", html);
            if (!enviado)
                _logger.LogWarning("No se pudo enviar el aviso de vacaciones (solicitud {Id}, nueva: {Nueva}).", idSolicitud, nueva);
        }
        catch (Exception ex)
        {
            // La solicitud ya quedó confirmada. Un fallo de correo no debe repetir la operación.
            _logger.LogError(ex, "Error enviando aviso de vacaciones (solicitud {Id}, nueva: {Nueva}).", idSolicitud, nueva);
        }
    }

    private async Task EnviarCorreosAprobacionAsync(long idSolicitud)
    {
        try
        {
            var solicitud = await ConsultaParaDocumentos().FirstAsync(s => s.IdSolicitud == idSolicitud);
            var datos = await ConstruirDatosDocumentosAsync(solicitud);
            var adjuntos = GenerarPdfs(datos).Select(d => new EmailAdjunto(d.NombreArchivo, d.Contenido)).ToList();

            var inicio = CalculosVacaciones.FechaLarga(datos.FechaInicio);
            var fin = CalculosVacaciones.FechaLarga(datos.FechaFin);
            var reingreso = CalculosVacaciones.FechaLarga(datos.FechaReingreso);

            var correoEmpleado = string.IsNullOrWhiteSpace(solicitud.Usuario.CorreoEmpresa)
                ? solicitud.Usuario.CorreoPersonal
                : solicitud.Usuario.CorreoEmpresa;

            var okEmpleado = await _emailService.SendEmailAsync(
                correoEmpleado,
                datos.NombreCompleto,
                "Tus vacaciones fueron aprobadas",
                PlantillasCorreo.VacacionesAprobadasEmpleado(
                    solicitud.Usuario.Nombre, inicio, fin, reingreso, datos.DiasGozados, datos.DiasPendientes),
                adjuntos);

            if (!okEmpleado)
                _logger.LogWarning("No se pudo enviar el correo de vacaciones aprobadas al empleado (solicitud {Id}).", idSolicitud);

            var personalRrhh = await _context.Usuarios
                .Where(x => x.Estado && x.Rol != null && x.Rol.Nombre == "RRHH")
                .ToListAsync();

            foreach (var rrhh in personalRrhh)
            {
                var correoRrhh = string.IsNullOrWhiteSpace(rrhh.CorreoEmpresa) ? rrhh.CorreoPersonal : rrhh.CorreoEmpresa;
                var ok = await _emailService.SendEmailAsync(
                    correoRrhh,
                    $"{rrhh.Nombre} {rrhh.Apellido}",
                    $"Vacaciones aprobadas: {datos.NombreCompleto}",
                    PlantillasCorreo.VacacionesAprobadasRrhh(
                        rrhh.Nombre, datos.NombreCompleto, inicio, fin, reingreso, datos.DiasGozados));

                if (!ok)
                    _logger.LogWarning("No se pudo enviar el aviso de vacaciones a RRHH ({Correo}, solicitud {Id}).", correoRrhh, idSolicitud);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar los correos de vacaciones aprobadas (solicitud {Id}).", idSolicitud);
        }
    }

    private IQueryable<SolicitudVacacion> ConsultaSolicitudesConIncludes()
        => _context.SolicitudesVacaciones
            .Include(s => s.Usuario)
            .Include(s => s.JefeAprobador);

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
        FechaSolicitud = s.FechaSolicitud
    };
}
