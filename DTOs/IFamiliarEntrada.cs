namespace Intranet.DTOs;

// Lo que comparten los DTOs de entrada de un familiar (alta y actualización),
// para aplicar las mismas reglas de parentesco y fecha de unión a ambos.
public interface IFamiliarEntrada
{
    string? Parentesco { get; set; }
    DateTime? FechaNacimiento { get; set; }
    DateTime? FechaUnion { get; set; }
}
