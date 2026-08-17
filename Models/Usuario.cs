using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Intranet.Models;

public class Usuario
{
    [Key]
    public long IdUsuario { get; set; }

    public long IdRol { get; set; }
    public long? IdCargo { get; set; }
    public long? IdCiudad { get; set; }
    public long? IdEstadoCivil { get; set; }
    public long? IdEtnia { get; set; }
    public long? IdGenero { get; set; }

    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string UsuarioNombre { get; set; } = null!; // Corresponde a la columna 'usuario'
    public string CorreoEmpresa { get; set; } = null!;

    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public string ContrasenaHash { get; set; } = null!;
    public bool DebeCambiarContrasena { get; set; } = true;

    public string? UrlImagenPerfil { get; set; }
    public int DiasVacacionesAsignados { get; set; } = 15;
    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    // Relaciones pertenencia
    public virtual Rol Rol { get; set; } = null!;
    public virtual Cargo? Cargo { get; set; }
    public virtual Ciudad? Ciudad { get; set; }
    public virtual EstadoCivil? EstadoCivil { get; set; }
    public virtual Etnia? Etnia { get; set; }
    public virtual Genero? Genero { get; set; }

    // Relaciones de colecciones
    public virtual ICollection<Familiar> Familiares { get; set; } = new List<Familiar>();
    public virtual ICollection<ContactoEmergencia> ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
    public virtual ICollection<DatoBancario> DatosBancarios { get; set; } = new List<DatoBancario>();
    public virtual ICollection<Titulo> Titulos { get; set; } = new List<Titulo>();
    public virtual ICollection<Vacacion> VacacionesRecibidas { get; set; } = new List<Vacacion>();
    public virtual ICollection<Vacacion> VacacionesRegistradas { get; set; } = new List<Vacacion>();
}