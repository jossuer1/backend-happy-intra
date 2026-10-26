using System.Text.Json;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Helpers;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public class SolicitudPerfilService : ISolicitudPerfilService
{
    // Si al rechazar el plazo original ya venció, se extiende este número de días
    // desde hoy para que el empleado tenga tiempo real de corregir.
    private const int DiasExtensionTrasRechazo = 7;

    private readonly AppDbContext _context;
    private readonly PerfilPropioAplicador _aplicador;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<SolicitudPerfilService> _logger;

    public SolicitudPerfilService(AppDbContext context, PerfilPropioAplicador aplicador,
        IEmailService emailService, IConfiguration config, ILogger<SolicitudPerfilService> logger)
    {
        _context = context;
        _aplicador = aplicador;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    // ------------------------------------------------------------------
    // Empleado: enviar
    // ------------------------------------------------------------------
    public async Task<ServiceResult<long>> EnviarAsync(long idUsuario, ActualizarPerfilPropioDto dto)
    {
        var usuario = await CargarUsuarioConSubrecursos(idUsuario);
        if (usuario == null)
            return ServiceResult<long>.Fallo("Usuario no encontrado.");

        if (!usuario.Estado)
            return ServiceResult<long>.Fallo("Tu cuenta está inactiva.");

        if (!usuario.VentanaEdicionAbierta)
            return ServiceResult<long>.Fallo(
                "No tienes habilitada la edición de tu perfil en este momento. Pídele a RRHH que la active.");

        if (PerfilPropioAplicador.EstaVacio(dto))
            return ServiceResult<long>.Fallo(
                "No enviaste ningún cambio. Si tu información ya está al día, confírmalo con «Mi información está al día».");

        if (await _context.SolicitudesCambioPerfil.AnyAsync(s =>
                s.IdUsuario == idUsuario && s.Estado == EstadoSolicitudCambioPerfil.Pendiente))
            return ServiceResult<long>.Fallo("Ya tienes una solicitud pendiente de revisión por RRHH.");

        // Validación "en seco": se aplican las reglas sobre las entidades en memoria y se
        // descartan sin guardar. Así el empleado se entera de los errores al enviar.
        var error = await _aplicador.AplicarAsync(usuario, dto);
        _context.ChangeTracker.Clear();
        if (error != null)
            return ServiceResult<long>.Fallo(error);

        var solicitud = new SolicitudCambioPerfil
        {
            IdUsuario = idUsuario,
            CambiosJson = JsonSerializer.Serialize(dto),
            Estado = EstadoSolicitudCambioPerfil.Pendiente,
            FechaSolicitud = DateTime.UtcNow
        };

        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.SolicitudesCambioPerfil.Add(solicitud);
            await _context.SaveChangesAsync();

            // La ventana se cierra al enviar: mientras RRHH revisa no se puede enviar otra.
            // (PuedeActualizarHasta se conserva por si hay que reabrirla tras un rechazo.)
            var ahora = DateTime.UtcNow;
            await _context.Usuarios.Where(u => u.IdUsuario == idUsuario)
                .ExecuteUpdateAsync(set => set
                    .SetProperty(u => u.PuedeActualizarPerfil, false)
                    .SetProperty(u => u.FechaActualizacion, ahora));

            await tx.CommitAsync();
        }
        catch (DbUpdateException)
        {
            // Índice único parcial: ya existe una solicitud pendiente de este usuario.
            return ServiceResult<long>.Fallo("Ya tienes una solicitud pendiente de revisión por RRHH.");
        }

        return ServiceResult<long>.Ok(solicitud.IdSolicitud);
    }

    // ------------------------------------------------------------------
    // Empleado: confirmar que no tiene cambios
    // ------------------------------------------------------------------
    public async Task<ServiceResult<long>> ConfirmarSinCambiosAsync(long idUsuario)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        if (usuario == null)
            return ServiceResult<long>.Fallo("Usuario no encontrado.");

        if (!usuario.Estado)
            return ServiceResult<long>.Fallo("Tu cuenta está inactiva.");

        if (!usuario.VentanaEdicionAbierta)
            return ServiceResult<long>.Fallo(
                "No tienes habilitada la edición de tu perfil en este momento. Pídele a RRHH que la active.");

        if (await _context.SolicitudesCambioPerfil.AnyAsync(s =>
                s.IdUsuario == idUsuario && s.Estado == EstadoSolicitudCambioPerfil.Pendiente))
            return ServiceResult<long>.Fallo("Ya tienes una solicitud pendiente de revisión por RRHH.");

        var ahora = DateTime.UtcNow;

        // Ya resuelta desde el inicio: no tiene revisor ni cambios que aplicar.
        var solicitud = new SolicitudCambioPerfil
        {
            IdUsuario = idUsuario,
            CambiosJson = "{}",
            Estado = EstadoSolicitudCambioPerfil.SinCambios,
            FechaSolicitud = ahora,
            FechaRevision = ahora
        };

        await using var tx = await _context.Database.BeginTransactionAsync();
        _context.SolicitudesCambioPerfil.Add(solicitud);
        await _context.SaveChangesAsync();

        // Responder cierra la ventana (igual que al enviar cambios o al ser aprobado).
        await _context.Usuarios.Where(u => u.IdUsuario == idUsuario)
            .ExecuteUpdateAsync(set => set
                .SetProperty(u => u.PuedeActualizarPerfil, false)
                .SetProperty(u => u.PuedeActualizarHasta, (DateTime?)null)
                .SetProperty(u => u.FechaActualizacion, ahora));

        await tx.CommitAsync();

        return ServiceResult<long>.Ok(solicitud.IdSolicitud);
    }

    // ------------------------------------------------------------------
    // Listados
    // ------------------------------------------------------------------
    public async Task<ServiceResult<List<SolicitudCambioPerfilResumenDto>>> ObtenerMiasAsync(long idUsuario)
    {
        var lista = await ConsultaResumen(_context.SolicitudesCambioPerfil.Where(s => s.IdUsuario == idUsuario))
            .ToListAsync();
        return ServiceResult<List<SolicitudCambioPerfilResumenDto>>.Ok(lista);
    }

    public async Task<ServiceResult<List<SolicitudCambioPerfilResumenDto>>> ListarAsync(string? estado)
    {
        var query = _context.SolicitudesCambioPerfil.AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            var e = estado.Trim().ToUpperInvariant();
            if (e != EstadoSolicitudCambioPerfil.Pendiente && e != EstadoSolicitudCambioPerfil.Aprobada
                && e != EstadoSolicitudCambioPerfil.Rechazada && e != EstadoSolicitudCambioPerfil.SinCambios)
                return ServiceResult<List<SolicitudCambioPerfilResumenDto>>.Fallo(
                    "Estado inválido. Usa PENDIENTE, APROBADA, RECHAZADA o SIN_CAMBIOS.");

            query = query.Where(s => s.Estado == e);
        }

        var lista = await ConsultaResumen(query).ToListAsync();
        return ServiceResult<List<SolicitudCambioPerfilResumenDto>>.Ok(lista);
    }

    private static IQueryable<SolicitudCambioPerfilResumenDto> ConsultaResumen(IQueryable<SolicitudCambioPerfil> query)
        => query
            .OrderByDescending(s => s.FechaSolicitud)
            .Select(s => new SolicitudCambioPerfilResumenDto
            {
                IdSolicitud = s.IdSolicitud,
                IdUsuario = s.IdUsuario,
                Empleado = s.Usuario.Nombre + " " + s.Usuario.Apellido,
                Cedula = s.Usuario.Cedula,
                Estado = s.Estado,
                FechaSolicitud = s.FechaSolicitud,
                Revisor = s.Revisor != null ? s.Revisor.Nombre + " " + s.Revisor.Apellido : null,
                FechaRevision = s.FechaRevision,
                ObservacionRrhh = s.ObservacionRrhh
            });

    // ------------------------------------------------------------------
    // Detalle con comparativo
    // ------------------------------------------------------------------
    public async Task<ServiceResult<SolicitudCambioPerfilDetalleDto>> ObtenerDetalleAsync(
        long idSolicitud, long idSolicitante, bool esRrhh)
    {
        var solicitud = await _context.SolicitudesCambioPerfil
            .AsNoTracking()
            .Include(s => s.Usuario)
            .Include(s => s.Revisor)
            .FirstOrDefaultAsync(s => s.IdSolicitud == idSolicitud);

        if (solicitud == null || (!esRrhh && solicitud.IdUsuario != idSolicitante))
            return ServiceResult<SolicitudCambioPerfilDetalleDto>.Fallo("Solicitud no encontrada.");

        var dto = JsonSerializer.Deserialize<ActualizarPerfilPropioDto>(solicitud.CambiosJson)
                  ?? new ActualizarPerfilPropioDto();

        // El "antes" solo tiene sentido mientras la solicitud sigue pendiente: una vez
        // resuelta, los datos actuales ya reflejan (o no) el cambio.
        var comparar = solicitud.Estado == EstadoSolicitudCambioPerfil.Pendiente;
        var actual = await _context.Usuarios
            .AsNoTracking()
            .Include(u => u.EstadoCivil)
            .Include(u => u.Familiares)
            .Include(u => u.ContactosEmergencia)
            .Include(u => u.Titulos)
            .AsSplitQuery()
            .FirstAsync(u => u.IdUsuario == solicitud.IdUsuario);

        var detalle = new SolicitudCambioPerfilDetalleDto
        {
            IdSolicitud = solicitud.IdSolicitud,
            IdUsuario = solicitud.IdUsuario,
            Empleado = solicitud.Usuario.Nombre + " " + solicitud.Usuario.Apellido,
            Cedula = solicitud.Usuario.Cedula,
            Estado = solicitud.Estado,
            FechaSolicitud = solicitud.FechaSolicitud,
            Revisor = solicitud.Revisor != null ? solicitud.Revisor.Nombre + " " + solicitud.Revisor.Apellido : null,
            FechaRevision = solicitud.FechaRevision,
            ObservacionRrhh = solicitud.ObservacionRrhh
        };

        void Campo(string nombre, string? anterior, string? nuevo)
        {
            if (nuevo == null) return;
            if (comparar && anterior == nuevo) return;
            detalle.Campos.Add(new CambioCampoDto
            {
                Campo = nombre,
                Anterior = comparar ? anterior : null,
                Nuevo = nuevo
            });
        }

        Campo("Celular personal", actual.CelularPersonal, dto.CelularPersonal);
        Campo("Celular de empresa", actual.CelularEmpresa, dto.CelularEmpresa);
        Campo("Dirección", actual.Direccion, dto.Direccion);
        Campo("Correo personal", actual.CorreoPersonal, dto.CorreoPersonal);

        if (dto.IdEstadoCivil.HasValue)
        {
            var nuevoEc = await _context.EstadosCiviles.AsNoTracking()
                .Where(e => e.IdEstadoCivil == dto.IdEstadoCivil.Value)
                .Select(e => e.Nombre)
                .FirstOrDefaultAsync();
            Campo("Estado civil", actual.EstadoCivil?.Nombre, nuevoEc ?? $"(id {dto.IdEstadoCivil})");
        }

        // --- Familiares ---
        foreach (var f in dto.Familiares ?? new())
        {
            var existente = f.IdFamiliar is long idF && idF > 0
                ? actual.Familiares.FirstOrDefault(x => x.IdFamiliar == idF) : null;
            detalle.Familiares.Add(new CambioItemDto
            {
                Accion = f.IdFamiliar is > 0 ? "EDITAR" : "AGREGAR",
                IdRegistro = f.IdFamiliar is > 0 ? f.IdFamiliar : null,
                Anterior = comparar && existente != null
                    ? DescribirFamiliar(existente.Nombre, existente.Apellido, existente.Parentesco,
                        existente.Cedula, existente.FechaNacimiento, existente.FechaUnion) : null,
                Nuevo = DescribirFamiliar(f.Nombre, f.Apellido, f.Parentesco, f.Cedula, f.FechaNacimiento, f.FechaUnion)
            });
        }
        foreach (var id in dto.FamiliaresAEliminar ?? new())
        {
            var existente = actual.Familiares.FirstOrDefault(x => x.IdFamiliar == id);
            detalle.Familiares.Add(new CambioItemDto
            {
                Accion = "ELIMINAR",
                IdRegistro = id,
                Anterior = existente != null
                    ? DescribirFamiliar(existente.Nombre, existente.Apellido, existente.Parentesco,
                        existente.Cedula, existente.FechaNacimiento, existente.FechaUnion)
                    : $"(id {id})"
            });
        }

        // --- Contactos de emergencia ---
        foreach (var c in dto.ContactosEmergencia ?? new())
        {
            var existente = c.IdContacto is long idC && idC > 0
                ? actual.ContactosEmergencia.FirstOrDefault(x => x.IdContacto == idC) : null;
            detalle.ContactosEmergencia.Add(new CambioItemDto
            {
                Accion = c.IdContacto is > 0 ? "EDITAR" : "AGREGAR",
                IdRegistro = c.IdContacto is > 0 ? c.IdContacto : null,
                Anterior = comparar && existente != null
                    ? DescribirContacto(existente.Nombre, existente.Apellido, existente.Parentesco,
                        existente.Telefono, existente.Direccion) : null,
                Nuevo = DescribirContacto(c.Nombre, c.Apellido, c.Parentesco, c.Telefono, c.Direccion)
            });
        }
        foreach (var id in dto.ContactosEmergenciaAEliminar ?? new())
        {
            var existente = actual.ContactosEmergencia.FirstOrDefault(x => x.IdContacto == id);
            detalle.ContactosEmergencia.Add(new CambioItemDto
            {
                Accion = "ELIMINAR",
                IdRegistro = id,
                Anterior = existente != null
                    ? DescribirContacto(existente.Nombre, existente.Apellido, existente.Parentesco,
                        existente.Telefono, existente.Direccion)
                    : $"(id {id})"
            });
        }

        // --- Títulos ---
        foreach (var t in dto.Titulos ?? new())
        {
            var existente = t.IdTitulo is long idT && idT > 0
                ? actual.Titulos.FirstOrDefault(x => x.IdTitulo == idT) : null;
            detalle.Titulos.Add(new CambioItemDto
            {
                Accion = t.IdTitulo is > 0 ? "EDITAR" : "AGREGAR",
                IdRegistro = t.IdTitulo is > 0 ? t.IdTitulo : null,
                Anterior = comparar && existente != null
                    ? DescribirTitulo(existente.NombreTitulo, existente.Institucion, existente.Culminado,
                        existente.FechaObtencion) : null,
                Nuevo = DescribirTitulo(t.NombreTitulo, t.Institucion, t.Culminado, t.FechaObtencion)
            });
        }
        foreach (var id in dto.TitulosAEliminar ?? new())
        {
            var existente = actual.Titulos.FirstOrDefault(x => x.IdTitulo == id);
            detalle.Titulos.Add(new CambioItemDto
            {
                Accion = "ELIMINAR",
                IdRegistro = id,
                Anterior = existente != null
                    ? DescribirTitulo(existente.NombreTitulo, existente.Institucion, existente.Culminado,
                        existente.FechaObtencion)
                    : $"(id {id})"
            });
        }

        return ServiceResult<SolicitudCambioPerfilDetalleDto>.Ok(detalle);
    }

    // ------------------------------------------------------------------
    // RRHH: aprobar
    // ------------------------------------------------------------------
    public async Task<ServiceResult<bool>> AprobarAsync(long idSolicitud, long idRevisor, string? observacion)
    {
        var solicitud = await _context.SolicitudesCambioPerfil.FirstOrDefaultAsync(s => s.IdSolicitud == idSolicitud);
        if (solicitud == null)
            return ServiceResult<bool>.Fallo("Solicitud no encontrada.");

        if (solicitud.Estado != EstadoSolicitudCambioPerfil.Pendiente)
            return ServiceResult<bool>.Fallo("Esta solicitud ya fue resuelta.");

        var usuario = await CargarUsuarioConSubrecursos(solicitud.IdUsuario);
        if (usuario == null)
            return ServiceResult<bool>.Fallo("El usuario de esta solicitud ya no existe.");

        var dto = JsonSerializer.Deserialize<ActualizarPerfilPropioDto>(solicitud.CambiosJson)
                  ?? new ActualizarPerfilPropioDto();

        // Se vuelve a validar: entre el envío y la aprobación los datos pudieron cambiar.
        var error = await _aplicador.AplicarAsync(usuario, dto);
        if (error != null)
        {
            _context.ChangeTracker.Clear();
            return ServiceResult<bool>.Fallo(
                $"No se pudieron aplicar los cambios: {error} Recházala indicando el motivo para que el empleado la corrija.");
        }

        var ahora = DateTime.UtcNow;
        usuario.PuedeActualizarPerfil = false;
        usuario.PuedeActualizarHasta = null;
        usuario.FechaActualizacion = ahora;

        solicitud.Estado = EstadoSolicitudCambioPerfil.Aprobada;
        solicitud.IdRevisor = idRevisor;
        solicitud.FechaRevision = ahora;
        solicitud.ObservacionRrhh = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();

        try
        {
            await _context.SaveChangesAsync(); // una sola SaveChanges = una sola transacción
        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<bool>.Fallo("Esta solicitud ya fue resuelta por otra persona.");
        }

        await EnviarCorreoAsync(usuario, "Tus cambios de perfil fueron aprobados",
            PlantillasCorreo.CambiosPerfilAprobados(usuario.Nombre, solicitud.ObservacionRrhh));

        return ServiceResult<bool>.Ok(true);
    }

    // ------------------------------------------------------------------
    // RRHH: rechazar
    // ------------------------------------------------------------------
    public async Task<ServiceResult<bool>> RechazarAsync(long idSolicitud, long idRevisor, string observacion)
    {
        if (string.IsNullOrWhiteSpace(observacion))
            return ServiceResult<bool>.Fallo("Debes indicar el motivo del rechazo.");

        var solicitud = await _context.SolicitudesCambioPerfil
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.IdSolicitud == idSolicitud);
        if (solicitud == null)
            return ServiceResult<bool>.Fallo("Solicitud no encontrada.");

        if (solicitud.Estado != EstadoSolicitudCambioPerfil.Pendiente)
            return ServiceResult<bool>.Fallo("Esta solicitud ya fue resuelta.");

        var ahora = DateTime.UtcNow;
        var usuario = solicitud.Usuario;

        solicitud.Estado = EstadoSolicitudCambioPerfil.Rechazada;
        solicitud.IdRevisor = idRevisor;
        solicitud.FechaRevision = ahora;
        solicitud.ObservacionRrhh = observacion.Trim();

        // Se le devuelve la ventana para que corrija y reenvíe sin que RRHH la reactive.
        // Si el plazo original ya venció, se extiende desde hoy.
        usuario.PuedeActualizarPerfil = true;
        if (usuario.PuedeActualizarHasta.HasValue && usuario.PuedeActualizarHasta.Value <= ahora)
            usuario.PuedeActualizarHasta = FechasEcuador.FinDelDiaEnUtc(
                FechasEcuador.Hoy().AddDays(DiasExtensionTrasRechazo));
        usuario.FechaActualizacion = ahora;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return ServiceResult<bool>.Fallo("Esta solicitud ya fue resuelta por otra persona.");
        }

        var limite = usuario.PuedeActualizarHasta.HasValue
            ? FechasEcuador.FechaLarga(usuario.PuedeActualizarHasta.Value)
            : "que RRHH indique";
        var urlIntranet = _config["Intranet:UrlBase"] ?? "https://intranet.happypay.com";

        await EnviarCorreoAsync(usuario, "Tus cambios de perfil no fueron aprobados",
            PlantillasCorreo.CambiosPerfilRechazados(usuario.Nombre, solicitud.ObservacionRrhh!, limite, urlIntranet));

        return ServiceResult<bool>.Ok(true);
    }

    // ------------------------------------------------------------------
    // Auxiliares
    // ------------------------------------------------------------------
    private Task<Usuario?> CargarUsuarioConSubrecursos(long idUsuario)
        => _context.Usuarios
            .Include(u => u.Familiares)
            .Include(u => u.ContactosEmergencia)
            .Include(u => u.Titulos)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

    // Los correos no deben deshacer la operación: si fallan, solo se registra en el log.
    private async Task EnviarCorreoAsync(Usuario usuario, string asunto, string html)
    {
        try
        {
            var correo = string.IsNullOrWhiteSpace(usuario.CorreoEmpresa) ? usuario.CorreoPersonal : usuario.CorreoEmpresa;
            if (string.IsNullOrWhiteSpace(correo)) return;

            var ok = await _emailService.SendEmailAsync(correo, $"{usuario.Nombre} {usuario.Apellido}", asunto, html);
            if (!ok)
                _logger.LogWarning("No se pudo enviar el correo '{Asunto}' a {Correo}.", asunto, correo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando el correo '{Asunto}' al usuario {Id}.", asunto, usuario.IdUsuario);
        }
    }

    private static string Unir(params string?[] partes)
        => string.Join(" · ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));

    private static string? Dia(DateTime? f) => f?.ToString("dd/MM/yyyy");

    private static string DescribirFamiliar(string nombre, string? apellido, string? parentesco,
        string? cedula, DateTime? nacimiento, DateTime? union)
        => Unir($"{nombre} {apellido}".Trim(), parentesco,
            string.IsNullOrWhiteSpace(cedula) ? null : $"C.I. {cedula}",
            nacimiento.HasValue ? $"Nac. {Dia(nacimiento)}" : null,
            union.HasValue ? $"Unión {Dia(union)}" : null);

    private static string DescribirContacto(string nombre, string? apellido, string? parentesco,
        string? telefono, string? direccion)
        => Unir($"{nombre} {apellido}".Trim(), parentesco, telefono, direccion);

    private static string DescribirTitulo(string nombre, string? institucion, bool culminado, DateTime? fecha)
        => Unir(nombre, institucion, culminado ? $"Culminado {Dia(fecha)}".Trim() : "En curso");
}
