using System.ComponentModel.DataAnnotations;

namespace Intranet.DTOs;

// Fila del listado de solicitudes (mías o, para RRHH, de todos).
public class SolicitudCambioPerfilResumenDto
{
    public long IdSolicitud { get; set; }
    public long IdUsuario { get; set; }
    public string Empleado { get; set; } = null!;
    public string Cedula { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public string? Revisor { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? ObservacionRrhh { get; set; }
}

// Un campo escalar que cambia (celular, dirección, correo personal, estado civil).
public class CambioCampoDto
{
    public string Campo { get; set; } = null!;
    public string? Anterior { get; set; }
    public string? Nuevo { get; set; }
}

// Un registro de una lista (familiar, contacto de emergencia, título) que se
// agrega, se edita o se elimina. Anterior/Nuevo vienen ya descritos en texto.
public class CambioItemDto
{
    // AGREGAR | EDITAR | ELIMINAR
    public string Accion { get; set; } = null!;
    public long? IdRegistro { get; set; }
    public string? Anterior { get; set; }
    public string? Nuevo { get; set; }
}

// Detalle para que RRHH revise: qué cambia respecto a los datos actuales.
// Si la solicitud ya fue resuelta, "Anterior" viene vacío (los datos actuales ya
// reflejan —o no— el cambio) y solo se muestra lo que se propuso.
public class SolicitudCambioPerfilDetalleDto : SolicitudCambioPerfilResumenDto
{
    public List<CambioCampoDto> Campos { get; set; } = new();
    public List<CambioItemDto> Familiares { get; set; } = new();
    public List<CambioItemDto> ContactosEmergencia { get; set; } = new();
    public List<CambioItemDto> Titulos { get; set; } = new();
}

public class ResolverSolicitudCambioPerfilDto
{
    // Opcional al aprobar; obligatoria al rechazar (es el motivo que recibe el empleado).
    [StringLength(500, ErrorMessage = "La observación no puede superar los 500 caracteres.")]
    public string? Observacion { get; set; }
}
