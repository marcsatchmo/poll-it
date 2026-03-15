using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Poll_it.Server.Data;
using Poll_it.Server.Hubs;
using Poll_it.Shared;

/// <summary>
/// Punto de entrada de la aplicación Poll-it Server.
/// Configura todos los servicios, middleware y endpoints de la API.
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SECCIÓN 1: CONFIGURACIÓN DE SERVICIOS
// ============================================================================

// Log del entorno actual para debugging
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"[STARTUP] Entorno: {environment}");

// Configurar Entity Framework Core con SQLite
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

// Configurar SignalR para comunicación en tiempo real
builder.Services.AddSignalR();

// Configurar CORS (Cross-Origin Resource Sharing)
// SOLUCIÓN ROBUSTA: El origen de producción está hardcodeado directamente aquí
// para garantizar que CORS funcione independientemente de si ASPNETCORE_ENVIRONMENT
// está configurado correctamente en Azure App Service.
// Los orígenes de appsettings se fusionan como valores adicionales.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        // Orígenes de producción hardcodeados como respaldo definitivo.
        // Esto garantiza que el frontend en Azure siempre pueda conectarse,
        // sin depender de que ASPNETCORE_ENVIRONMENT sea "Production".
        var hardcodedOrigins = new[]
        {
            "https://orange-moss-00032b10f.1.azurestaticapps.net",
            "https://localhost:7219",
            "http://localhost:5048"
        };

        // Orígenes adicionales leídos desde la configuración (appsettings.*.json)
        var configOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        // Fusionar ambas listas eliminando duplicados
        var allowedOrigins = hardcodedOrigins
            .Concat(configOrigins)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Console.WriteLine($"[CORS CONFIG] Entorno: '{environment}' — Orígenes permitidos ({allowedOrigins.Length}):");
        foreach (var origin in allowedOrigins)
        {
            Console.WriteLine($"  - {origin}");
        }

        policy.WithOrigins(allowedOrigins)
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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PainPointDbContext>();
    db.Database.EnsureCreated();
}

// ============================================================================
// SECCIÓN 3: CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE
// ============================================================================
// ORDEN CRÍTICO: UseCors DEBE ir antes de MapHub y los endpoints.

// Middleware de debugging CORS: registra si el header fue agregado o no
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        Console.WriteLine($"[CORS DEBUG] Petición recibida — Origen: {origin} | Método: {context.Request.Method} | Path: {context.Request.Path}");
    }

    await next();

    if (context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
    {
        var acoHeader = context.Response.Headers["Access-Control-Allow-Origin"];
        Console.WriteLine($"[CORS DEBUG] ✓ ACEPTADO — Access-Control-Allow-Origin: {acoHeader}");
    }
    else if (!string.IsNullOrEmpty(origin))
    {
        Console.WriteLine($"[CORS DEBUG] ✗ RECHAZADO — Origen '{origin}' no tiene Access-Control-Allow-Origin. Status: {context.Response.StatusCode}");
    }
});

// Habilitar CORS con la política nombrada.
// CRÍTICO: app.UseCors() DEBE llamarse con el nombre exacto de la política
// y ANTES de app.MapHub / app.MapGet / app.MapPost.
app.UseCors("AllowBlazorClient");

// Mapear el Hub de SignalR
app.MapHub<PainHub>("/painhub");

// ============================================================================
// SECCIÓN 4: DEFINICIÓN DE ENDPOINTS (MINIMAL API)
// ============================================================================

/// <summary>
/// GET /api/painpoints
/// Obtiene todos los puntos de dolor ordenados por fecha de creación (más recientes primero).
/// </summary>
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
/// Crea un nuevo punto de dolor y notifica a todos los clientes vía SignalR.
/// </summary>
app.MapPost("/api/painpoints", async (PainPointRequest request, PainPointDbContext db, IHubContext<PainHub> hubContext) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
    {
        return Results.BadRequest(new { error = "El texto del dolor no puede estar vacío" });
    }

    var colors = new[] { "yellow", "pink", "blue", "green", "purple", "orange" };

    var painPoint = new PainPoint
    {
        Text = request.Text.Trim(),
        CreatedAt = DateTime.UtcNow,
        Color = colors[Random.Shared.Next(colors.Length)]
    };

    db.PainPoints.Add(painPoint);
    await db.SaveChangesAsync();

    // Notificar a todos los clientes conectados vía SignalR en tiempo real
    await hubContext.Clients.All.SendAsync("ReceiveNewPain", painPoint);

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

/// <summary>
/// Modelo para la petición de creación de un punto de dolor.
/// </summary>
public record PainPointRequest(string Text);
