using System.Security.Claims;
using Intranet.Models;

namespace Intranet.Services;

public static class AccesoToken
{
    public static bool EsVigente(ClaimsPrincipal principal, Usuario? usuario)
    {
        if (usuario == null || !usuario.Estado) return false;
        // Compatibilidad con tokens anteriores a la migración: versión inicial 0.
        var versionClaim = principal.FindFirstValue("version_acceso");
        long version = 0;
        if (versionClaim != null && !long.TryParse(versionClaim, out version)) return false;
        if (version != usuario.VersionAcceso) return false;
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        if (usuario.IdRol == null) return roles.Length == 0;
        return usuario.Rol is { Estado: true } && roles.Length == 1 && roles[0] == usuario.Rol.Nombre;
    }
}
