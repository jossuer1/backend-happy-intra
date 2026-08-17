namespace Intranet.Models;

using System.ComponentModel.DataAnnotations;
public class DatoBancario
{
    [Key]
    public long IdDatoBancario { get; set; }
    public long IdUsuario { get; set; }
    public long IdBanco { get; set; }
    public string NumeroCuenta { get; set; } = null!;
    public string TipoCuenta { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Banco Banco { get; set; } = null!;
}