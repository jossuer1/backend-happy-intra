namespace Intranet.DTOs;

// --- CIUDAD ---
public class CiudadCrearDto
{
    public string Nombre { get; set; } = null!;
    public long IdProvincia { get; set; }
}

public class CiudadReadDto
{
    public long IdCiudad { get; set; }
    public string Nombre { get; set; } = null!;
    public long IdProvincia { get; set; }
    public bool Estado { get; set; }
    public ProvinciaSimpleDto? Provincia { get; set; }
}
public class ProvinciaSimpleDto
{
    public long IdProvincia { get; set; }
    public string Nombre { get; set; } = null!;
    public long IdRegion { get; set; }
}

public class CiudadActualizarDto
{
    public string Nombre { get; set; } = null!;
    public long IdProvincia { get; set; }
    public bool Estado { get; set; } = true;
}

// --- ETNIA ---
public class EtniaCrearDto
{
    public string Nombre { get; set; } = null!;
}

public class EtniaActualizarDto
{
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;
}

// --- GENERO ---
public class GeneroCrearDto
{
    public string Nombre { get; set; } = null!;
}

public class GeneroActualizarDto
{
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;
}

// --- ESTADO CIVIL ---
public class EstadoCivilCrearDto
{
    public string Nombre { get; set; } = null!;
}

public class EstadoCivilActualizarDto
{
    public string Nombre { get; set; } = null!;
    public bool Estado { get; set; } = true;
}