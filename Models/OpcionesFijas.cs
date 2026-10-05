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


// Nacionalidades permitidas del colaborador (Latinoamérica). Se guardan en femenino
// ("ECUATORIANA") y el acta de vacaciones las adapta al género con Gentilicio().
// Para sumar una nacionalidad nueva basta con agregarla a la lista (en femenino).
public static class Nacionalidades
{
    public const string Ecuatoriana = "ECUATORIANA";

    // Ecuatoriana va primero (es el valor por defecto); el resto, en orden alfabético.
    public static readonly string[] Todas =
    {
        Ecuatoriana,
        "ARGENTINA", "BOLIVIANA", "BRASILEÑA", "CHILENA", "COLOMBIANA",
        "COSTARRICENSE", "CUBANA", "DOMINICANA", "GUATEMALTECA", "HAITIANA",
        "HONDUREÑA", "MEXICANA", "NICARAGÜENSE", "PANAMEÑA", "PARAGUAYA",
        "PERUANA", "PUERTORRIQUEÑA", "SALVADOREÑA", "URUGUAYA", "VENEZOLANA"
    };

    // Femenino -> gentilicio según el género: "CHILENA" -> "chileno" (hombre) / "chilena" (mujer).
    // Los que terminan en -E (COSTARRICENSE, NICARAGÜENSE) son invariables y se devuelven tal cual.
    public static string Gentilicio(string? nacionalidad, bool esMujer)
    {
        var valor = string.IsNullOrWhiteSpace(nacionalidad) ? Ecuatoriana : nacionalidad.Trim();
        valor = valor.ToLowerInvariant();

        if (!esMujer && valor.EndsWith('a'))
            valor = valor[..^1] + "o";

        return valor;
    }
}
