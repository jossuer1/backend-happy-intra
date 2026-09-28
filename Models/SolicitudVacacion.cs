using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models;

// Estados posibles del flujo de aprobación de una solicitud de vacaciones.
// El flujo siempre es: el empleado solicita -> su jefe directo responde ->
// si el jefe aprueba, pasa a RRHH -> RRHH da la aprobación final (y ahí
// recién se descuentan los días del saldo, vía un registro en Vacaciones).
public static class EstadoSolicitudVacacion
{
    public const string PendienteJefe = "PENDIENTE_JEFE";
    public const string PendienteRrhh = "PENDIENTE_RRHH";
    public const string Aprobada = "APROBADA";
    public const string RechazadaJefe = "RECHAZADA_JEFE";
    public const string RechazadaRrhh = "RECHAZADA_RRHH";
}

public class SolicitudVacacion
{
    [Key]
    public long IdSolicitud { get; set; }

    // --- Quién solicita ---
    public long IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario Usuario { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int DiasSolicitados { get; set; }
    public string Motivo { get; set; } = null!;

    public string Estado { get; set; } = EstadoSolicitudVacacion.PendienteJefe;

    // --- Respuesta del jefe directo (primer nivel de aprobación) ---
    public long? IdJefeAprobador { get; set; }

    [ForeignKey(nameof(IdJefeAprobador))]
    public virtual Usuario? JefeAprobador { get; set; }

    public DateTime? FechaRespuestaJefe { get; set; }
    public string? ObservacionJefe { get; set; }

    // --- Respuesta de RRHH (segundo y último nivel de aprobación) ---
    public long? IdRrhhAprobador { get; set; }

    [ForeignKey(nameof(IdRrhhAprobador))]
    public virtual Usuario? RrhhAprobador { get; set; }

    public DateTime? FechaRespuestaRrhh { get; set; }
    public string? ObservacionRrhh { get; set; }

    // Una vez que RRHH aprueba, se genera automáticamente el movimiento de
    // descuento en Vacaciones; queda enlazado aquí para trazabilidad.
    public long? IdVacacionGenerada { get; set; }

    [ForeignKey(nameof(IdVacacionGenerada))]
    public virtual Vacacion? VacacionGenerada { get; set; }

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
}
