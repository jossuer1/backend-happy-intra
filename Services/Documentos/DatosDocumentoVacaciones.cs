namespace Intranet.Services.Documentos;

// Todos los datos que necesitan los 3 PDFs, ya calculados. Los PDFs solo dibujan.
public class DatosDocumentoVacaciones
{
    public string RazonSocial { get; set; } = "HAPPYPAY S.A.";
    public string RepresentanteLegal { get; set; } = "";

    public string NombreCompleto { get; set; } = "";
    public string Cedula { get; set; } = "";
    public string Area { get; set; } = "";
    public string Cargo { get; set; } = "";
    public DateTime? FechaIngreso { get; set; }
    public bool EsMujer { get; set; }
    public string Nacionalidad { get; set; } = "ECUATORIANA";

    public string NombreJefe { get; set; } = "";
    public string Ciudad { get; set; } = "Quito";

    // Fecha en que el jefe aprobó (hora Ecuador): es la fecha de emisión de los documentos.
    public DateTime FechaEmision { get; set; }

    public string PeriodoLaboral { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaReingreso { get; set; }

    public int DiasGozados { get; set; }           // días de esta solicitud
    public int DiasDisponiblesAntes { get; set; }  // saldo antes de descontar
    public int DiasPendientes { get; set; }        // saldo después de descontar

    public string Observacion { get; set; } = "";

    // Mes (y año) del rol de pagos en el que se refleja el pago: el mes de inicio de las vacaciones.
    public string MesRol { get; set; } = "";
    public int AnioRol { get; set; }
}
