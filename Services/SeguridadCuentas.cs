using Intranet.Data;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public static class SeguridadCuentas
{
    // Mismo primer bloqueo que vacaciones: serializa cambios de permisos y jerarquía.
    public static Task BloquearAsync(AppDbContext db) => db.Database.ExecuteSqlRawAsync(
        "LOCK TABLE \"Usuarios\", \"Roles\", \"SolicitudesVacaciones\" IN SHARE ROW EXCLUSIVE MODE");

    public static async Task<string?> ValidarRetiroAsync(AppDbContext db, long idUsuario, long? idRol)
    {
        var rol = await db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.IdRol == idRol);
        if (rol == null || !rol.Estado || rol.Nombre is not ("ADMIN" or "RRHH")) return null;
        return await db.Usuarios.AnyAsync(u => u.IdUsuario != idUsuario && u.Estado && u.IdRol == idRol)
            ? null : $"No puedes retirar el acceso del último usuario activo con rol {rol.Nombre}.";
    }
}
