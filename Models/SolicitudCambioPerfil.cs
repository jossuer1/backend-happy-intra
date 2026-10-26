using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models;

// Estados de una solicitud de cambio de perfil.
// Flujo: RRHH habilita la ventana de edición -> el empleado envía sus cambios
// (quedan PENDIENTE, nada se toca todavía) -> RRHH los aprueba (se aplican al
// usuario) o los rechaza (no se aplica nada y se le devuelve la ventana al
// empleado para que corrija y reenvíe). Si el empleado revisa y no tiene nada que
// cambiar, lo confirma y queda como SIN_CAMBIOS (ya resuelta, sin revisión de RRHH).
public static class EstadoSolicitudCambioPerfil
{
    public const string Pendiente = "PENDIENTE";
    public const string Aprobada = "APROBADA";
    public const string Rechazada = "RECHAZADA";
    public const string SinCambios = "SIN_CAMBIOS";
}

public class SolicitudCambioPerfil
{
    [Key]
    public long IdSolicitud { get; set; }

    // --- Quién pide el cambio ---
    public long IdUsuario { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario Usuario { get; set; } = null!;

    // Lo que el empleado propone, serializado (ActualizarPerfilPropioDto ya normalizado
    // y validado). Se guarda como jsonb: así la tabla no necesita espejos de Familiar,
    // ContactoEmergencia y Titulo.
    public string CambiosJson { get; set; } = null!;

    public string Estado { get; set; } = EstadoSolicitudCambioPerfil.Pendiente;

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

    // --- Respuesta de RRHH ---
    public long? IdRevisor { get; set; }

    [ForeignKey(nameof(IdRevisor))]
    public virtual Usuario? Revisor { get; set; }

    public DateTime? FechaRevision { get; set; }

    // Obligatoria al rechazar (es el "por qué" que se le envía al empleado).
    public string? ObservacionRrhh { get; set; }

    // xmin de PostgreSQL (token de concurrencia, ver AppDbContext).
    public uint Version { get; set; }
}
