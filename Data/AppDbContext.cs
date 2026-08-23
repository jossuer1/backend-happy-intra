using Intranet.Models; // Namespace donde tienes tus clases Area, Cargo, Banco, Usuario
using Microsoft.EntityFrameworkCore;

namespace Intranet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tablas / DbSets
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Banco> Bancos { get; set; }

    public DbSet<Rol> Roles { get; set; }
    public DbSet<Ciudad> Ciudades { get; set; }
    public DbSet<Provincia> Provincias { get; set; }
    public DbSet<Region> Regiones { get; set; }
    public DbSet<Etnia> Etnias { get; set; }
    public DbSet<Genero> Generos { get; set; }
    public DbSet<EstadoCivil> EstadosCiviles { get; set; }
    public DbSet<Vacacion> Vacaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vacacion>()
        .HasOne(v => v.Usuario)
        .WithMany(u => u.VacacionesRecibidas)
        .HasForeignKey(v => v.IdUsuario)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Vacacion>()
        .HasOne(v => v.RegistradoPor)
        .WithMany(u => u.VacacionesRegistradas)
        .HasForeignKey(v => v.IdRegistradoPor)
        .OnDelete(DeleteBehavior.Restrict);

        // 1. Áreas Únicas
        modelBuilder.Entity<Area>().HasData(
            new Area { IdArea = 1, Nombre = "Gerencial" },
            new Area { IdArea = 2, Nombre = "Tecnología" },
            new Area { IdArea = 3, Nombre = "Crédito" },
            new Area { IdArea = 4, Nombre = "Financiero" },
            new Area { IdArea = 5, Nombre = "Comercial" },
            new Area { IdArea = 6, Nombre = "Talento Humano" },
            new Area { IdArea = 7, Nombre = "Marketing" },
            new Area { IdArea = 8, Nombre = "Administrativo" }
        );

        // 2. Cargos Únicos asignados a sus Áreas correspondientes
        modelBuilder.Entity<Cargo>().HasData(
            new Cargo { IdCargo = 1, Nombre = "Gerente General", IdArea = 1 },            // Gerencial
            new Cargo { IdCargo = 2, Nombre = "Analista de Procesos", IdArea = 2 },          // Tecnología
            new Cargo { IdCargo = 3, Nombre = "Jefe de Crédito", IdArea = 3 },               // Crédito
            new Cargo { IdCargo = 4, Nombre = "Gerente de Operaciones", IdArea = 2 },        // Tecnología
            new Cargo { IdCargo = 5, Nombre = "Ingeniero en Infraestructura", IdArea = 2 },  // Tecnología
            new Cargo { IdCargo = 6, Nombre = "Asistente Contable", IdArea = 4 },            // Financiero
            new Cargo { IdCargo = 7, Nombre = "Ejecutivo de Cuentas", IdArea = 5 },          // Comercial
            new Cargo { IdCargo = 8, Nombre = "Promotor", IdArea = 5 },                      // Comercial
            new Cargo { IdCargo = 9, Nombre = "Asistente de Infraestructura", IdArea = 2 },  // Tecnología
            new Cargo { IdCargo = 10, Nombre = "Jefe de Recursos Humanos", IdArea = 6 },     // Talento Humano
            new Cargo { IdCargo = 11, Nombre = "Coordinador de Negocios", IdArea = 5 },      // Comercial
            new Cargo { IdCargo = 12, Nombre = "Analista de Datos", IdArea = 3 },            // Crédito
            new Cargo { IdCargo = 13, Nombre = "Monitor de Cobranza", IdArea = 3 },          // Crédito
            new Cargo { IdCargo = 14, Nombre = "Community Manager", IdArea = 7 },            // Marketing
            new Cargo { IdCargo = 15, Nombre = "Asistente de Crédito", IdArea = 3 },         // Crédito
            new Cargo { IdCargo = 16, Nombre = "Auxiliar de Servicios", IdArea = 8 },        // Administrativo
            new Cargo { IdCargo = 17, Nombre = "Servicio al Cliente", IdArea = 3 },          // Crédito
            new Cargo { IdCargo = 18, Nombre = "Contador General", IdArea = 4 },             // Financiero
            new Cargo { IdCargo = 19, Nombre = "Jefe de Negocios", IdArea = 5 },             // Comercial
            new Cargo { IdCargo = 20, Nombre = "Pasante de Sistemas", IdArea = 2 },          // Tecnología
            new Cargo { IdCargo = 21, Nombre = "Pasante de Talento Humano", IdArea = 6 },   // Talento Humano
            new Cargo { IdCargo = 22, Nombre = "Pasante de Diseño", IdArea = 7 },            // Marketing
            new Cargo { IdCargo = 23, Nombre = "Analista Contable", IdArea = 4 }             // Financiero
        );
        // 3. Bancos y Cooperativas del Ecuador
        modelBuilder.Entity<Banco>().HasData(
            new Banco { IdBanco = 1, Nombre = "Banco Pichincha" },
            new Banco { IdBanco = 2, Nombre = "Banco Guayaquil" },
            new Banco { IdBanco = 3, Nombre = "Banco del Pacífico" },
            new Banco { IdBanco = 4, Nombre = "Produbanco" },
            new Banco { IdBanco = 5, Nombre = "Banco Internacional" },
            new Banco { IdBanco = 6, Nombre = "Banco del Austro" },
            new Banco { IdBanco = 7, Nombre = "Banco Bolivariano" },
            new Banco { IdBanco = 8, Nombre = "Banco Solidario" },
            new Banco { IdBanco = 9, Nombre = "Banco General Rumiñahui" },
            new Banco { IdBanco = 10, Nombre = "Banco de Machala" },
            new Banco { IdBanco = 11, Nombre = "Banco de Loja" },
            new Banco { IdBanco = 12, Nombre = "Banco Diners Club" },
            new Banco { IdBanco = 13, Nombre = "Cooperativa JEP" },
            new Banco { IdBanco = 14, Nombre = "Cooperativa Policía Nacional" },
            new Banco { IdBanco = 15, Nombre = "Cooperativa Alianza del Valle" },
            new Banco { IdBanco = 16, Nombre = "Cooperativa Andalucía" },
            new Banco { IdBanco = 17, Nombre = "Cooperativa San Francisco" },
            new Banco { IdBanco = 18, Nombre = "Mutualista Pichincha" }
        );
        // 4. Regiones del Ecuador
        modelBuilder.Entity<Region>().HasData(
            new Region { IdRegion = 1, Nombre = "Costa" },
            new Region { IdRegion = 2, Nombre = "Sierra" },
            new Region { IdRegion = 3, Nombre = "Amazonía" },
            new Region { IdRegion = 4, Nombre = "Insular" }
        );

        // 5. Provincias del Ecuador (24), asociadas a su región
        modelBuilder.Entity<Provincia>().HasData(
            // Costa
            new Provincia { IdProvincia = 1, Nombre = "Esmeraldas", IdRegion = 1 },
            new Provincia { IdProvincia = 2, Nombre = "Manabí", IdRegion = 1 },
            new Provincia { IdProvincia = 3, Nombre = "Santo Domingo de los Tsáchilas", IdRegion = 1 },
            new Provincia { IdProvincia = 4, Nombre = "Los Ríos", IdRegion = 1 },
            new Provincia { IdProvincia = 5, Nombre = "Guayas", IdRegion = 1 },
            new Provincia { IdProvincia = 6, Nombre = "Santa Elena", IdRegion = 1 },
            new Provincia { IdProvincia = 7, Nombre = "El Oro", IdRegion = 1 },
            // Sierra
            new Provincia { IdProvincia = 8, Nombre = "Carchi", IdRegion = 2 },
            new Provincia { IdProvincia = 9, Nombre = "Imbabura", IdRegion = 2 },
            new Provincia { IdProvincia = 10, Nombre = "Pichincha", IdRegion = 2 },
            new Provincia { IdProvincia = 11, Nombre = "Cotopaxi", IdRegion = 2 },
            new Provincia { IdProvincia = 12, Nombre = "Tungurahua", IdRegion = 2 },
            new Provincia { IdProvincia = 13, Nombre = "Bolívar", IdRegion = 2 },
            new Provincia { IdProvincia = 14, Nombre = "Chimborazo", IdRegion = 2 },
            new Provincia { IdProvincia = 15, Nombre = "Cañar", IdRegion = 2 },
            new Provincia { IdProvincia = 16, Nombre = "Azuay", IdRegion = 2 },
            new Provincia { IdProvincia = 17, Nombre = "Loja", IdRegion = 2 },
            // Amazonía
            new Provincia { IdProvincia = 18, Nombre = "Sucumbíos", IdRegion = 3 },
            new Provincia { IdProvincia = 19, Nombre = "Napo", IdRegion = 3 },
            new Provincia { IdProvincia = 20, Nombre = "Orellana", IdRegion = 3 },
            new Provincia { IdProvincia = 21, Nombre = "Pastaza", IdRegion = 3 },
            new Provincia { IdProvincia = 22, Nombre = "Morona Santiago", IdRegion = 3 },
            new Provincia { IdProvincia = 23, Nombre = "Zamora Chinchipe", IdRegion = 3 },
            // Insular
            new Provincia { IdProvincia = 24, Nombre = "Galápagos", IdRegion = 4 }
        );
    }

}