namespace Intranet.DTOs;

public record ResumenRrhhDto(int EmpleadosActivos, int Areas, int SolicitudesPendientes);

public record CumpleanosProximoDto(
    long IdUsuario, string NombreCompleto, string? Foto, DateOnly FechaCumpleanos,
    int EdadQueCumple, bool YaFelicitado);
