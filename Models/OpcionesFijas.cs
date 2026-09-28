namespace Intranet.Models;

// Listas fijas que se validan en el backend (sin tabla ni CRUD, igual que
// EstadoSolicitudVacacion). Para agregar un valor nuevo basta con sumarlo aquí.
// El frontend las obtiene desde GET /api/Catalogos/opciones-fijas para armar
// sus combos, así los valores nunca quedan duplicados a mano en dos lugares.

public static class TiposContrato
{
    public const string Indefinido = "INDEFINIDO";
    public const string Emergente = "EMERGENTE";
    public const string Productivo = "PRODUCTIVO";

    public static readonly string[] Todos = { Indefinido, Emergente, Productivo };

    // Solo estos tipos exigen (y admiten) fecha de fin de contrato.
    public static readonly string[] ConFechaFin = { Emergente, Productivo };

    public static bool RequiereFechaFin(string? tipo) => tipo != null && ConFechaFin.Contains(tipo);
}

public static class Jornadas
{
    public const string TiempoCompleto = "TIEMPO COMPLETO";
    public const string TiempoParcial = "TIEMPO PARCIAL";

    public static readonly string[] Todas = { TiempoCompleto, TiempoParcial };
}

// Parentescos permitidos en los familiares del empleado (no aplica a los
// contactos de emergencia, cuyo parentesco sigue siendo texto libre).
public static class Parentescos
{
    public const string Conyuge = "CONYUGE";
    public const string Hijo = "HIJO";

    public static readonly string[] Todos = { Conyuge, Hijo };
}
