using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Models;

public class Usuario
{
    [Key]
    public long IdUsuario { get; set; }

    // --- Llaves Foráneas (IDs) ---
    public long? IdRol { get; set; }
    public long? IdCargo { get; set; }
    public long? IdCiudad { get; set; }
    public long? IdEstadoCivil { get; set; }
    public long? IdEtnia { get; set; }
    public long? IdGenero { get; set; }

    // --- Propiedades Escalares ---
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string UsuarioNombre { get; set; } = null!; // Corresponde a la columna 'usuario'
    public string CorreoEmpresa { get; set; } = null!;
    public string CorreoPersonal { get; set; } = null!;

    public string? CelularPersonal { get; set; }
    public string? CelularEmpresa { get; set; }
    public string? Direccion { get; set; }

    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public string ContrasenaHash { get; set; } = null!;
    public bool DebeCambiarContrasena { get; set; } = true;

    public string? UrlImagenPerfil { get; set; }

    // --- Beneficio de Vacaciones ---
    // No todos los colaboradores tienen derecho a vacaciones (ej. pasantes, honorarios).
    public bool TieneVacaciones { get; set; } = true;
    public int DiasVacacionesAsignados { get; set; } = 15;

    public bool Estado { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    // --- Relaciones de Pertenencia 
    [ForeignKey(nameof(IdRol))]
    public virtual Rol? Rol { get; set; }

    [ForeignKey(nameof(IdCargo))]
    public virtual Cargo? Cargo { get; set; }

    [ForeignKey(nameof(IdCiudad))]
    public virtual Ciudad? Ciudad { get; set; }

    [ForeignKey(nameof(IdEstadoCivil))]
    public virtual EstadoCivil? EstadoCivil { get; set; }

    [ForeignKey(nameof(IdEtnia))]
    public virtual Etnia? Etnia { get; set; }

    [ForeignKey(nameof(IdGenero))]
    public virtual Genero? Genero { get; set; }

    // --- Relaciones de Colecciones ---
    public virtual ICollection<Familiar> Familiares { get; set; } = new List<Familiar>();
    public virtual ICollection<ContactoEmergencia> ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
    public virtual ICollection<DatoBancario> DatosBancarios { get; set; } = new List<DatoBancario>();
    public virtual ICollection<Titulo> Titulos { get; set; } = new List<Titulo>();
    public virtual ICollection<Vacacion> VacacionesRecibidas { get; set; } = new List<Vacacion>();
    public virtual ICollection<Vacacion> VacacionesRegistradas { get; set; } = new List<Vacacion>();
}