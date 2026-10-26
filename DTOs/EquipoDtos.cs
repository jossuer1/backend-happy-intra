using System.ComponentModel.DataAnnotations;

namespace Intranet.DTOs;

public class TraspasarEquipoDto
{
    [Range(1, long.MaxValue)]
    public long IdJefeAnterior { get; set; }

    [Range(1, long.MaxValue)]
    public long IdJefeNuevo { get; set; }
}

public class TraspasarEquipoResultadoDto
{
    public long IdJefeAnterior { get; set; }
    public long IdJefeNuevo { get; set; }
    public int EmpleadosTrasladados { get; set; }
    public int SolicitudesPendientesTrasladadas { get; set; }
}
