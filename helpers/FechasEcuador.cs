namespace Intranet.Helpers;

// Ecuador continental está en UTC-5 todo el año (sin horario de verano), así que
// se usa un desfase fijo en lugar de TimeZoneInfo (evita depender de tzdata en el contenedor).
public static class FechasEcuador
{
    private static readonly TimeSpan Desfase = TimeSpan.FromHours(-5);

    public static DateTime Ahora() => DateTime.UtcNow + Desfase;

    public static DateTime Hoy() => Ahora().Date;

    // Último instante (23:59:59) del día indicado, hora de Ecuador, devuelto en UTC
    // (la columna es timestamptz y Npgsql exige Kind=Utc).
    public static DateTime FinDelDiaEnUtc(DateTime dia)
    {
        var finLocal = dia.Date.AddDays(1).AddSeconds(-1);
        return DateTime.SpecifyKind(finLocal - Desfase, DateTimeKind.Utc);
    }

    // Texto largo para correos: "5 de noviembre de 2026".
    public static string FechaLarga(DateTime utc)
    {
        var l = utc + Desfase;
        string[] meses = { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio",
                           "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
        return $"{l.Day} de {meses[l.Month - 1]} de {l.Year}";
    }
}
