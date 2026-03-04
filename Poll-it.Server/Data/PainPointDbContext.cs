using Microsoft.EntityFrameworkCore;
using Poll_it.Shared;

namespace Poll_it.Server.Data;

/// <summary>
/// Contexto de base de datos para la aplicación Poll-it.
/// Gestiona la persistencia de los puntos de dolor usando Entity Framework Core con SQLite.
/// </summary>
/// <remarks>
/// Arquitectura - Capa de Datos:
/// - El DbContext actúa como nuestro "puente" entre el código C# y la base de datos SQLite.
/// - Encapsula todas las operaciones de base de datos, manteniendo la lógica de persistencia
///   separada de la lógica de negocio (Separation of Concerns).
/// - SQLite fue elegido por ser portable y no requerir instalación de servidor de BD,
///   perfecto para demostraciones y desarrollo.
/// </remarks>
public class PainPointDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe opciones de configuración.
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto (proveedor, connection string, etc.)</param>
    /// <remarks>
    /// Las opciones se configuran en Program.cs mediante Dependency Injection,
    /// siguiendo el patrón de configuración de ASP.NET Core.
    /// </remarks>
    public PainPointDbContext(DbContextOptions<PainPointDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Colección de puntos de dolor en la base de datos.
    /// </summary>
    /// <remarks>
    /// Este DbSet representa la tabla "PainPoints" en la base de datos.
    /// Entity Framework lo usa para:
    /// - Generar consultas SQL automáticamente
    /// - Realizar el seguimiento de cambios
    /// - Ejecutar operaciones CRUD (Create, Read, Update, Delete)
    /// </remarks>
    public DbSet<PainPoint> PainPoints => Set<PainPoint>();

    /// <summary>
    /// Configura el modelo de datos y las relaciones.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo de Entity Framework</param>
    /// <remarks>
    /// Aquí definimos la configuración específica de cada entidad:
    /// - Nombres de tablas
    /// - Índices
    /// - Restricciones
    /// - Valores por defecto
    /// En nuestro caso, configuramos propiedades importantes del PainPoint.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad PainPoint
        modelBuilder.Entity<PainPoint>(entity =>
        {
            // Definir el nombre de la tabla
            entity.ToTable("PainPoints");

            // Configurar la clave primaria
            entity.HasKey(p => p.Id);

            // El Id se genera automáticamente (auto-incremento en SQLite)
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // El texto es requerido y tiene un límite de 500 caracteres
            entity.Property(p => p.Text)
                .IsRequired()
                .HasMaxLength(500);

            // La fecha de creación tiene un valor por defecto
            entity.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("datetime('now')");

            // El color tiene un valor por defecto
            entity.Property(p => p.Color)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("yellow");

            // Crear un índice en CreatedAt para consultas ordenadas por fecha
            entity.HasIndex(p => p.CreatedAt);
        });
    }
}
