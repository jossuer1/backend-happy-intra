using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Intranet.Models;

namespace Intranet.Helpers;

public static class ValidacionesExpediente
{
    public static DateTime Hoy => DateTime.UtcNow.AddHours(-5).Date;
    public static bool FechaInformada(DateTime? fecha) => fecha.HasValue && fecha.Value.Date >= new DateTime(1900, 1, 1);
    public static bool CelularValido(string? valor) => string.IsNullOrWhiteSpace(valor) || Regex.IsMatch(valor.Trim(), @"^09[0-9]{8}$");
    public static bool TelefonoValido(string? valor) => !string.IsNullOrWhiteSpace(valor) && Regex.IsMatch(valor.Trim(), @"^[0-9]{7,10}$");
    public static bool NombreValido(string? valor) => !string.IsNullOrWhiteSpace(valor) &&
        valor.Trim().Length is >= 2 and <= 50 && Regex.IsMatch(valor.Trim(), @"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$");

    public static string? Validar(Usuario u, bool expedienteCompleto = true)
    {
        if (!CelularValido(u.CelularPersonal) || !CelularValido(u.CelularEmpresa))
            return "Los celulares deben tener 10 dígitos y comenzar con 09.";
        if (string.IsNullOrWhiteSpace(u.CorreoPersonal) || !new EmailAddressAttribute().IsValid(u.CorreoPersonal))
            return "El correo personal es obligatorio y debe tener un formato válido.";
        if (expedienteCompleto)
        {
            if (!NombreValido(u.Nombre) || !NombreValido(u.Apellido)) return "El nombre y apellido deben tener entre 2 y 50 caracteres y contener letras.";
            if (string.IsNullOrWhiteSpace(u.CorreoEmpresa) || !new EmailAddressAttribute().IsValid(u.CorreoEmpresa))
                return "El correo empresarial es obligatorio y debe tener un formato válido.";
            if (u.IdCargo is null or <= 0) return "Selecciona un cargo válido.";
            if (!FechaInformada(u.FechaIngreso)) return "La fecha de ingreso es obligatoria y debe ser válida.";
            if (u.FechaNacimiento.HasValue && (!FechaInformada(u.FechaNacimiento) || u.FechaNacimiento.Value.Date > Hoy))
                return "La fecha de nacimiento no puede ser futura ni anterior a 1900.";
            if (u.FechaNacimiento.HasValue && u.FechaIngreso!.Value.Date <= u.FechaNacimiento.Value.Date)
                return "La fecha de ingreso debe ser posterior al nacimiento.";
            if (u.DiasVacacionesAsignados < 0 || u.DiasVacacionesAsignados > 365)
                return "Los días de vacaciones asignados deben estar entre 0 y 365.";
            if (!u.DatosBancarios.Any(b => b.Estado)) return "Registra al menos una cuenta bancaria.";
            if (u.DatosBancarios.Where(b => b.Estado).GroupBy(b => (b.IdBanco, b.NumeroCuenta?.Trim())).Any(g => g.Count() > 1))
                return "No puedes registrar dos veces la misma cuenta del mismo banco en el expediente.";
            foreach (var b in u.DatosBancarios.Where(b => b.Estado))
            {
                if (b.TipoCuenta is not ("AHORROS" or "CORRIENTE")) return "El tipo de cuenta debe ser AHORROS o CORRIENTE.";
                if (!Regex.IsMatch(b.NumeroCuenta ?? "", @"^[0-9]{3,30}$")) return "La cuenta bancaria debe contener entre 3 y 30 dígitos.";
            }
            foreach (var p in u.PeriodosIess)
                if (!FechaInformada(p.FechaIngreso) || p.FechaSalida.HasValue && !FechaInformada(p.FechaSalida) ||
                    u.FechaNacimiento.HasValue && p.FechaIngreso.Date <= u.FechaNacimiento.Value.Date)
                    return "Las fechas de los períodos IESS deben ser válidas y posteriores al nacimiento.";
        }
        if (!u.ContactosEmergencia.Any(c => c.Estado)) return "Registra al menos un contacto de emergencia.";
        foreach (var c in u.ContactosEmergencia.Where(c => c.Estado))
            if (!NombreValido(c.Nombre) || !string.IsNullOrWhiteSpace(c.Apellido) && !NombreValido(c.Apellido) || string.IsNullOrWhiteSpace(c.Parentesco) || !TelefonoValido(c.Telefono))
                return "Cada contacto requiere nombre, parentesco y un teléfono de 7 a 10 dígitos.";
        foreach (var f in u.Familiares.Where(f => f.Estado))
        {
            if (!NombreValido(f.Nombre) || !string.IsNullOrWhiteSpace(f.Apellido) && !NombreValido(f.Apellido)) return "Los nombres de familiares deben contener letras y tener entre 2 y 50 caracteres.";
            var fecha = f.Parentesco == Parentescos.Conyuge ? f.FechaUnion : f.FechaNacimiento;
            if (!FechaInformada(fecha) || fecha!.Value.Date > Hoy)
                return f.Parentesco == Parentescos.Conyuge
                    ? "El cónyuge requiere una fecha de unión válida que no sea futura."
                    : "Cada hijo requiere una fecha de nacimiento válida que no sea futura.";
            if (f.Parentesco == Parentescos.Conyuge && u.FechaNacimiento.HasValue && fecha.Value.Date <= u.FechaNacimiento.Value.Date)
                return "La fecha de unión debe ser posterior al nacimiento del colaborador.";
        }
        foreach (var t in u.Titulos.Where(t => t.Estado))
            if (t.FechaObtencion.HasValue && (!FechaInformada(t.FechaObtencion) || t.FechaObtencion.Value.Date > Hoy ||
                u.FechaNacimiento.HasValue && t.FechaObtencion.Value.Date <= u.FechaNacimiento.Value.Date))
                return "La fecha de obtención del título debe ser válida, posterior al nacimiento y no futura.";
        return null;
    }
}

// A diferencia de Required, detecta el valor por defecto de un DateTime no anulable.
public sealed class FechaInformadaAttribute : ValidationAttribute
{
    public FechaInformadaAttribute() : base("Indica una fecha válida a partir de 1900.") { }
    public override bool IsValid(object? value) => value == null || value is DateTime fecha && ValidacionesExpediente.FechaInformada(fecha);
}
