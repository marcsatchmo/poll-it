using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Poll_it.Server.Data;
using Poll_it.Server.Hubs;
using Poll_it.Shared;

/// <summary>
/// Punto de entrada de la aplicación Poll-it Server.
/// Configura todos los servicios, middleware y endpoints de la API.
/// </summary>
/// <remarks>
/// Arquitectura - Configuración de la Aplicación:
/// 
/// Este archivo sigue el patrón de "Minimal API" de .NET, que permite configurar
/// toda la aplicación de manera concisa y legible. La estructura es:
/// 
/// 1. CONFIGURACIÓN DE SERVICIOS (builder.Services):
///    - DbContext para acceso a datos
///    - SignalR para comunicación en tiempo real
///    - CORS para permitir llamadas desde el cliente Blazor
/// 
/// 2. CONSTRUCCIÓN DE LA APLICACIÓN (builder.Build())
/// 
/// 3. CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE:
///    - Orden importante: cada middleware procesa la petición en secuencia
///    - CORS debe ir antes de los endpoints
///    - SignalR Hub se mapea para establecer las conexiones WebSocket
/// 
/// 4. DEFINICIÓN DE ENDPOINTS (Minimal API):
///    - Endpoints RESTful para operaciones CRUD
///    - Cada endpoint está documentado y sigue principios REST
/// 
/// Este diseño ejemplifica "Separation of Concerns": 
/// - La configuración está separada de la lógica de negocio
/// - Cada componente tiene una responsabilidad clara
/// - Es fácil de entender y modificar
/// </remarks>

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SECCIÓN 1: CONFIGURACIÓN DE SERVICIOS
// ============================================================================
// Los servicios se registran aquí y estarán disponibles mediante 
// Dependency Injection en toda la aplicación.

// Configurar Entity Framework Core con SQLite
// La cadena de conexión especifica que la BD se guardará en el archivo "painpoints.db"
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

// Configurar SignalR para comunicación en tiempo real
// SignalR maneja automáticamente las conexiones WebSocket, long polling, etc.
builder.Services.AddSignalR();

// Configurar CORS (Cross-Origin Resource Sharing)
// Necesario porque el cliente Blazor (puerto diferente) hará peticiones al servidor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7219", 
            "http://localhost:5048")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Requerido para SignalR
    });
});

// ============================================================================
// SECCIÓN 2: CONSTRUCCIÓN DE LA APLICACIÓN
// ============================================================================

var app = builder.Build();

// Asegurar que la base de datos esté creada
// En producción, usarías migraciones, pero para demos esto es más simple
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PainPointDbContext>();
    db.Database.EnsureCreated();
}

// ============================================================================
// SECCIÓN 3: CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE
// ============================================================================
// El orden importa: cada petición pasa por estos middleware en secuencia

// Habilitar CORS (debe ir antes de los endpoints)
app.UseCors("AllowBlazorClient");

// Mapear el Hub de SignalR
// Los clientes se conectarán a "/painhub" para recibir actualizaciones en tiempo real
app.MapHub<PainHub>("/painhub");

// ============================================================================
// SECCIÓN 4: DEFINICIÓN DE ENDPOINTS (MINIMAL API)
// ============================================================================
// Aquí definimos nuestra API RESTful de manera simple y directa

/// <summary>
/// GET /api/painpoints
/// Obtiene todos los puntos de dolor ordenados por fecha de creación (más recientes primero).
/// </summary>
/// <remarks>
/// Este endpoint se llama cuando se carga la página para mostrar todos los puntos existentes.
/// Arquitectura: Separación entre la capa de datos (DbContext) y la capa de presentación (API).
/// </remarks>
app.MapGet("/api/painpoints", async (PainPointDbContext db) =>
{
    var painPoints = await db.PainPoints
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
    
    return Results.Ok(painPoints);
})
.WithName("GetAllPainPoints")
.WithOpenApi();

/// <summary>
/// POST /api/painpoints
/// Crea un nuevo punto de dolor y notifica a todos los clientes conectados vía SignalR.
/// </summary>
/// <remarks>
/// Flujo de la operación:
/// 1. Recibe el texto del dolor del cliente
/// 2. Crea un nuevo PainPoint con un color aleatorio
/// 3. Lo guarda en la base de datos
/// 4. Usa SignalR para notificar a TODOS los clientes conectados en tiempo real
/// 
/// Este es el corazón de la arquitectura de tiempo real de la aplicación.
/// </remarks>
app.MapPost("/api/painpoints", async (PainPointRequest request, PainPointDbContext db, IHubContext<PainHub> hubContext) =>
{
    // Validar que el texto no esté vacío
    if (string.IsNullOrWhiteSpace(request.Text))
    {
        return Results.BadRequest(new { error = "El texto del dolor no puede estar vacío" });
    }

    // Colores disponibles para los Post-its
    var colors = new[] { "yellow", "pink", "blue", "green", "purple", "orange" };
    
    // Crear el nuevo punto de dolor
    var painPoint = new PainPoint
    {
        Text = request.Text.Trim(),
        CreatedAt = DateTime.UtcNow,
        Color = colors[Random.Shared.Next(colors.Length)]
    };

    // Guardar en la base de datos
    db.PainPoints.Add(painPoint);
    await db.SaveChangesAsync();

    // ¡MOMENTO CLAVE! Notificar a todos los clientes conectados vía SignalR
    // Este es el "momento mágico" donde todos ven aparecer el nuevo dolor en tiempo real
    await hubContext.Clients.All.SendAsync("ReceiveNewPain", painPoint);

    // Retornar el punto creado con su ID
    return Results.Created($"/api/painpoints/{painPoint.Id}", painPoint);
})
.WithName("CreatePainPoint")
.WithOpenApi();

// ============================================================================
// SECCIÓN 5: INICIAR LA APLICACIÓN
// ============================================================================

app.Run();

// ============================================================================
// MODELOS DE PETICIÓN
// ============================================================================
// Estos son DTOs (Data Transfer Objects) para las peticiones HTTP

/// <summary>
/// Modelo para la petición de creación de un punto de dolor.
/// </summary>
/// <remarks>
/// Usamos un record para simplicidad. Es inmutable y tiene igualdad por valor.
/// </remarks>
public record PainPointRequest(string Text);
