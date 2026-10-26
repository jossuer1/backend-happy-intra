using Intranet.Data;
using Intranet.DTOs;
using Intranet.Helpers;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

// Aplica sobre un Usuario (ya cargado con Familiares, ContactosEmergencia y Titulos)
// los cambios que el propio empleado propuso. NO guarda: solo modifica las entidades
// que EF ya está siguiendo y devuelve un mensaje de error (o null si todo es válido).
//
// Se usa en dos momentos con las mismas reglas:
//   1. Al ENVIAR la solicitud: para validar y fallar rápido. Después se descartan los
//      cambios con ChangeTracker.Clear(), sin llegar a guardar nada.
//   2. Al APROBAR: para aplicar de verdad (y guardar). Se vuelve a validar porque los
//      datos pudieron cambiar desde el envío (ej. RRHH borró un familiar a editar).
public class PerfilPropioAplicador
{
    private readonly AppDbContext _context;

    public PerfilPropioAplicador(AppDbContext context)
    {
        _context = context;
    }

    // true si la solicitud no propone absolutamente nada.
    public static bool EstaVacio(ActualizarPerfilPropioDto d)
        => d.CelularPersonal == null && d.CelularEmpresa == null && d.Direccion == null
           && d.CorreoPersonal == null && d.IdEstadoCivil == null
           && (d.Familiares?.Count ?? 0) == 0 && (d.ContactosEmergencia?.Count ?? 0) == 0
           && (d.Titulos?.Count ?? 0) == 0 && (d.FamiliaresAEliminar?.Count ?? 0) == 0
           && (d.ContactosEmergenciaAEliminar?.Count ?? 0) == 0 && (d.TitulosAEliminar?.Count ?? 0) == 0;

    // Normaliza los textos del DTO (mayúsculas, recortes). Es idempotente: se puede
    // llamar al enviar y de nuevo al aprobar sin alterar el resultado.
    public static void Normalizar(ActualizarPerfilPropioDto dto)
    {
        dto.CelularEmpresa = TextoHelper.SoloTrim(dto.CelularEmpresa);
        dto.CelularPersonal = TextoHelper.SoloTrim(dto.CelularPersonal);
        dto.Direccion = TextoHelper.AMayusculas(dto.Direccion);
        if (dto.CorreoPersonal != null)
            dto.CorreoPersonal = TextoHelper.AEmailNormalizado(dto.CorreoPersonal);

        if (dto.Familiares != null)
            foreach (var f in dto.Familiares)
            {
                f.Nombre = TextoHelper.AMayusculasObligatorio(f.Nombre);
                f.Apellido = TextoHelper.AMayusculas(f.Apellido);
                f.Parentesco = TextoHelper.AMayusculas(f.Parentesco);
                f.Cedula = UsuarioService.NormalizarCedulaOpcional(f.Cedula);
            }

        if (dto.ContactosEmergencia != null)
            foreach (var c in dto.ContactosEmergencia)
            {
                c.Nombre = TextoHelper.AMayusculasObligatorio(c.Nombre);
                c.Apellido = TextoHelper.AMayusculas(c.Apellido);
                c.Parentesco = TextoHelper.AMayusculas(c.Parentesco);
                c.Telefono = TextoHelper.SoloTrim(c.Telefono);
                c.Direccion = TextoHelper.AMayusculas(c.Direccion);
            }

        if (dto.Titulos != null)
            foreach (var t in dto.Titulos)
            {
                t.NombreTitulo = TextoHelper.AMayusculasObligatorio(t.NombreTitulo);
                t.Institucion = TextoHelper.AMayusculas(t.Institucion);
            }
    }

    public async Task<string?> AplicarAsync(Usuario usuario, ActualizarPerfilPropioDto dto)
    {
        Normalizar(dto);

        var errorFamiliares = UsuarioService.NormalizarYValidarFamiliares(dto.Familiares);
        if (errorFamiliares != null)
            return errorFamiliares;

        // --- Campos escalares (null = no cambiar; "" = vaciar, salvo el correo) ---
        if (dto.CelularEmpresa != null) usuario.CelularEmpresa = dto.CelularEmpresa;
        if (dto.CelularPersonal != null) usuario.CelularPersonal = dto.CelularPersonal;
        if (dto.Direccion != null) usuario.Direccion = dto.Direccion;

        if (dto.CorreoPersonal != null)
        {
            if (dto.CorreoPersonal.Length == 0)
                return "El correo personal no puede quedar vacío.";

            if (dto.CorreoPersonal != usuario.CorreoPersonal &&
                await _context.Usuarios.AnyAsync(u =>
                    u.CorreoPersonal == dto.CorreoPersonal && u.IdUsuario != usuario.IdUsuario))
                return "El correo personal ya está registrado en otro usuario.";

            usuario.CorreoPersonal = dto.CorreoPersonal;
        }

        if (dto.IdEstadoCivil.HasValue)
        {
            if (!await _context.EstadosCiviles.AnyAsync(e => e.IdEstadoCivil == dto.IdEstadoCivil.Value && e.Estado))
                return "El estado civil seleccionado no existe.";

            usuario.IdEstadoCivil = dto.IdEstadoCivil.Value;
        }

        // --- Familiares (mismo upsert por Id que usa RRHH) ---
        if (dto.FamiliaresAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Familiares.Where(f => dto.FamiliaresAEliminar.Contains(f.IdFamiliar)).ToList();
            foreach (var f in aEliminar)
            {
                usuario.Familiares.Remove(f);
                _context.Remove(f);
            }
        }

        if (dto.Familiares is { Count: > 0 })
        {
            foreach (var f in dto.Familiares)
            {
                if (f.IdFamiliar is long idF && idF > 0)
                {
                    var existente = usuario.Familiares.FirstOrDefault(x => x.IdFamiliar == idF);
                    if (existente == null)
                        return $"El familiar con id {idF} ya no existe en tu perfil.";

                    existente.Nombre = f.Nombre;
                    existente.Apellido = f.Apellido;
                    existente.Parentesco = f.Parentesco;
                    existente.Cedula = f.Cedula;
                    existente.FechaNacimiento = UsuarioService.ComoUtc(f.FechaNacimiento);
                    existente.FechaUnion = UsuarioService.ComoUtc(f.FechaUnion);
                }
                else
                {
                    usuario.Familiares.Add(new Familiar
                    {
                        Nombre = f.Nombre,
                        Apellido = f.Apellido,
                        Parentesco = f.Parentesco,
                        Cedula = f.Cedula,
                        FechaNacimiento = UsuarioService.ComoUtc(f.FechaNacimiento),
                        FechaUnion = UsuarioService.ComoUtc(f.FechaUnion),
                        Estado = true
                    });
                }
            }
        }

        // Un empleado solo puede tener un cónyuge (se revisa sobre el resultado final).
        if (usuario.Familiares.Count(f => f.Estado && f.Parentesco == Parentescos.Conyuge) > 1)
            return "Un empleado solo puede tener un cónyuge registrado.";

        var errorCedulas = UsuarioService.ValidarCedulasFamiliares(
            usuario.Familiares.Where(f => f.Estado).Select(f => f.Cedula), usuario.Cedula);
        if (errorCedulas != null)
            return errorCedulas;

        // --- Contactos de emergencia ---
        if (dto.ContactosEmergenciaAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.ContactosEmergencia
                .Where(c => dto.ContactosEmergenciaAEliminar.Contains(c.IdContacto)).ToList();
            foreach (var c in aEliminar)
            {
                usuario.ContactosEmergencia.Remove(c);
                _context.Remove(c);
            }
        }

        if (dto.ContactosEmergencia is { Count: > 0 })
        {
            foreach (var c in dto.ContactosEmergencia)
            {
                if (c.IdContacto is long idC && idC > 0)
                {
                    var existente = usuario.ContactosEmergencia.FirstOrDefault(x => x.IdContacto == idC);
                    if (existente == null)
                        return $"El contacto de emergencia con id {idC} ya no existe en tu perfil.";

                    existente.Nombre = c.Nombre;
                    existente.Apellido = c.Apellido;
                    existente.Parentesco = c.Parentesco;
                    existente.Telefono = c.Telefono;
                    existente.Direccion = c.Direccion;
                }
                else
                {
                    usuario.ContactosEmergencia.Add(new ContactoEmergencia
                    {
                        Nombre = c.Nombre,
                        Apellido = c.Apellido,
                        Parentesco = c.Parentesco,
                        Telefono = c.Telefono,
                        Direccion = c.Direccion,
                        Estado = true
                    });
                }
            }
        }

        // --- Títulos (nuevo: lo educativo también lo puede proponer el empleado) ---
        if (dto.TitulosAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Titulos.Where(t => dto.TitulosAEliminar.Contains(t.IdTitulo)).ToList();
            foreach (var t in aEliminar)
            {
                usuario.Titulos.Remove(t);
                _context.Remove(t);
            }
        }

        if (dto.Titulos is { Count: > 0 })
        {
            foreach (var t in dto.Titulos)
            {
                if (t.IdTitulo is long idT && idT > 0)
                {
                    var existente = usuario.Titulos.FirstOrDefault(x => x.IdTitulo == idT);
                    if (existente == null)
                        return $"El título con id {idT} ya no existe en tu perfil.";

                    existente.NombreTitulo = t.NombreTitulo;
                    existente.Institucion = t.Institucion;
                    existente.Culminado = t.Culminado;
                    existente.FechaObtencion = t.Culminado ? UsuarioService.ComoUtc(t.FechaObtencion) : null;
                }
                else
                {
                    usuario.Titulos.Add(new Titulo
                    {
                        NombreTitulo = t.NombreTitulo,
                        Institucion = t.Institucion,
                        Culminado = t.Culminado,
                        FechaObtencion = t.Culminado ? UsuarioService.ComoUtc(t.FechaObtencion) : null,
                        Estado = true
                    });
                }
            }
        }

        return ValidacionesExpediente.Validar(usuario, expedienteCompleto: false);
    }
}
