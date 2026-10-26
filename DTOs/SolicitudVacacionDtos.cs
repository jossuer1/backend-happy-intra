using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

// El empleado la crea para pedir vacaciones. El IdUsuario NO viene en el
// body: se toma del token del usuario logueado, para que nadie pueda pedir
// vacaciones "en nombre de otro".
public class SolicitudVacacionCrearDto
{
    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [FechaInformada]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    [FechaInformada]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [NoSoloEspacios]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre 3 y 200 caracteres.")]
    public string Motivo { get; set; } = null!;
}

// Lo usa el jefe directo para aprobar o rechazar.
public class RespuestaSolicitudVacacionDto
{
    [Required(ErrorMessage = "Debes indicar si apruebas o rechazas la solicitud.")]
    public bool? Aprobar { get; set; }

    [StringLength(200, ErrorMessage = "La observación no puede superar los 200 caracteres.")]
    public string? Observacion { get; set; }
}

public class SolicitudVacacionDto
{
    public long IdSolicitud { get; set; }
    public long IdUsuario { get; set; }
    public string SolicitanteNombre { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int DiasSolicitados { get; set; }
    public string Motivo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? JefeAprobadorNombre { get; set; }
    public DateTime? FechaRespuestaJefe { get; set; }
    public string? ObservacionJefe { get; set; }

    public DateTime FechaSolicitud { get; set; }
}
