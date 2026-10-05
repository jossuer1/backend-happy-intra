using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models;

// Estados del flujo de una solicitud de vacaciones.
// El flujo es: el empleado solicita -> su jefe directo responde. Si el jefe
// aprueba, ahí mismo se descuentan los días del saldo (registro en Vacaciones),
// se generan los 3 documentos y se avisa por correo al empleado y a RRHH.
// RRHH NO aprueba nada: solo recibe después los documentos firmados.
public static class EstadoSolicitudVacacion
{
    public const string PendienteJefe = "PENDIENTE_JEFE";
    public const string Aprobada = "APROBADA";
    public const string RechazadaJefe = "RECHAZADA_JEFE";
    // La solicitud estaba aprobada pero RRHH anuló el descuento (vacaciones canceladas).
    public const string Anulada = "ANULADA";
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

    // --- Respuesta del jefe directo (única aprobación del flujo) ---
    public long? IdJefeAprobador { get; set; }

    [ForeignKey(nameof(IdJefeAprobador))]
    public virtual Usuario? JefeAprobador { get; set; }

    public DateTime? FechaRespuestaJefe { get; set; }
    public string? ObservacionJefe { get; set; }

    // Cuando el jefe aprueba se genera el movimiento de descuento en Vacaciones;
    // queda enlazado aquí para trazabilidad.
    public long? IdVacacionGenerada { get; set; }

    [ForeignKey(nameof(IdVacacionGenerada))]
    public virtual Vacacion? VacacionGenerada { get; set; }

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
}
