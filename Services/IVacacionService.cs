using Intranet.DTOs;

namespace Intranet.Services;

public interface IVacacionService
{
    Task<ServiceResult<VacacionDto>> RegistrarDescuentoAsync(VacacionDescuentoCrearDto dto, long idRegistradoPor);
    Task<ServiceResult<VacacionDto>> RegistrarAjusteAsync(VacacionAjusteCrearDto dto, long idRegistradoPor);
    Task<ServiceResult<SaldoVacacionesDto>> ObtenerSaldoAsync(long idUsuario);
    Task<ServiceResult<List<VacacionDto>>> ObtenerHistorialAsync(long idUsuario);
    Task<ServiceResult<List<VacacionDto>>> ObtenerTodasLasVacacionesAsync();
    Task<ServiceResult<List<ResumenVacacionesDto>>> ObtenerResumenAsync();
}