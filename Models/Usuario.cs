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
    public long? IdTipoSangre { get; set; }

    // Jefe directo del empleado: quien resuelve el primer nivel de sus
    // solicitudes de vacaciones antes de que pasen a RRHH.
    public long? IdJefeDirecto { get; set; }

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

    // Ventana de autoedición: por defecto el usuario NO puede tocar su propio
    // perfil (todo pasa por RRHH). RRHH la activa puntualmente (PATCH
    // /api/Usuarios/{id}/permiso-actualizacion) para que el empleado pueda
    // editar sus propios datos una vez; al guardar, se vuelve a desactivar sola.
    public bool PuedeActualizarPerfil { get; set; } = false;

    public string? UrlImagenPerfil { get; set; }

    // --- Beneficio de Vacaciones ---
    // No todos los colaboradores tienen derecho a vacaciones (ej. pasantes, honorarios).
    public bool TieneVacaciones { get; set; } = true;
    public int DiasVacacionesAsignados { get; set; } = 15;

    // --- Condición laboral ---
    // Cargo tal como consta en la afiliación del IESS (puede diferir del cargo en Happy Pay).
    public string? CargoIess { get; set; }

    // Valores permitidos en Models/OpcionesFijas.cs (Jornadas y TiposContrato).
    public string? Jornada { get; set; }
    public string? TipoContrato { get; set; }

    // Solo aplica a los tipos de contrato que vencen (EMERGENTE / PRODUCTIVO).
    public DateTime? FechaFinContrato { get; set; }

    public bool RecibeComisiones { get; set; } = false;
    public bool AcumulaDecimos { get; set; } = false;

    // Habilita a este usuario para ser elegido como jefe directo de otros (y, por
    // tanto, para aprobar solicitudes de vacaciones). Es una marca aparte del rol
    // porque cada usuario tiene un solo rol y un colaborador de RRHH también puede
    // ser jefe. Solo RRHH la modifica.
    public bool EsJefe { get; set; } = false;

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

    [ForeignKey(nameof(IdTipoSangre))]
    public virtual TipoSangre? TipoSangre { get; set; }

    [ForeignKey(nameof(IdJefeDirecto))]
    public virtual Usuario? JefeDirecto { get; set; }

    // Empleados que reportan directamente a este usuario.
    public virtual ICollection<Usuario> Subordinados { get; set; } = new List<Usuario>();

    // --- Relaciones de Colecciones ---
    public virtual ICollection<Familiar> Familiares { get; set; } = new List<Familiar>();
    public virtual ICollection<ContactoEmergencia> ContactosEmergencia { get; set; } = new List<ContactoEmergencia>();
    public virtual ICollection<DatoBancario> DatosBancarios { get; set; } = new List<DatoBancario>();
    public virtual ICollection<Titulo> Titulos { get; set; } = new List<Titulo>();
    public virtual ICollection<Vacacion> VacacionesRecibidas { get; set; } = new List<Vacacion>();
    public virtual ICollection<Vacacion> VacacionesRegistradas { get; set; } = new List<Vacacion>();
    public virtual ICollection<SolicitudVacacion> SolicitudesVacaciones { get; set; } = new List<SolicitudVacacion>();
}