namespace Intranet.Models;

public class FelicitacionCumpleanos
{
    public long IdUsuario { get; set; }
    public int Anio { get; set; }
    public DateTime FechaEnvio { get; set; }

    // Los envíos automáticos no tienen un remitente de RRHH.
    public long? IdUsuarioRRHH { get; set; }
}
