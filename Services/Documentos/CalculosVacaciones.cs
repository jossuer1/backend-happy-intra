namespace Intranet.Services.Documentos;

// Reglas de cálculo usadas por los documentos de vacaciones. Están todas aquí,
// en un solo lugar, para que si RRHH cambia alguna regla solo se toque este archivo.
public static class CalculosVacaciones
{
    // Días de vacaciones que se ganan por cada año de servicio (Código del Trabajo, art. 69).
    public const int DiasPorPeriodo = 15;

    // Fijos en código (no CultureInfo("es-EC")) para que no dependan de ICU/globalización del contenedor.
    private static readonly string[] Meses =
    {
        "enero", "febrero", "marzo", "abril", "mayo", "junio",
        "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
    };

    public static string NombreMes(int mes) => Meses[mes - 1];

    // "11 de agosto de 2026"
    public static string FechaLarga(DateTime f) => $"{f.Day} de {NombreMes(f.Month)} de {f.Year}";

    // Fecha y hora de Ecuador (UTC-5) a partir de un instante UTC guardado en BD.
    public static DateTime AHoraEcuador(DateTime utc)
    {
        TimeZoneInfo tz;
        try { tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"); }
        catch (TimeZoneNotFoundException) { tz = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"); }
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
    }

    // Día de regreso al trabajo: el día siguiente al fin de las vacaciones, pero si cae
    // en fin de semana se pasa al lunes. (Los feriados no se consideran: no hay tabla de feriados.)
    public static DateTime FechaReingreso(DateTime fechaFin)
    {
        var f = fechaFin.Date.AddDays(1);
        while (f.DayOfWeek == DayOfWeek.Saturday || f.DayOfWeek == DayOfWeek.Sunday)
            f = f.AddDays(1);
        return f;
    }

    // Período laboral que se está tomando: el más antiguo que todavía tenía días
    // pendientes. Se deduce de cuántos días ya había consumido el empleado ANTES de
    // esta solicitud (cada período vale 15 días). Ej.: ingresó en 2024 y no había
    // tomado nada -> "2024-2025"; ya tomó 15 días -> "2025-2026".
    public static string PeriodoLaboral(DateTime? fechaIngreso, int diasConsumidosAntes)
    {
        if (fechaIngreso == null) return "";
        int indice = Math.Max(0, diasConsumidosAntes) / DiasPorPeriodo;
        int anioInicio = fechaIngreso.Value.Year + indice;
        return $"{anioInicio}-{anioInicio + 1}";
    }
}
