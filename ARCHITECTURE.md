# 🏛️ Poll-it - Documentación de Arquitectura

## Índice
1. [Visión General](#visión-general)
2. [Principios Arquitectónicos](#principios-arquitectónicos)
3. [Componentes del Sistema](#componentes-del-sistema)
4. [Flujos de Comunicación](#flujos-de-comunicación)
5. [Decisiones de Diseño](#decisiones-de-diseño)
6. [Patrones Implementados](#patrones-implementados)

---

## Visión General

Poll-it es una aplicación de captura de "dolores" técnicos en tiempo real que ejemplifica arquitectura de software moderna. Fue diseñada específicamente para **demostrar conceptos arquitectónicos** en un contexto educativo.

### Objetivos Arquitectónicos

1. **Claridad sobre Complejidad**: Código legible que prioriza la comprensión
2. **Separación de Concerns**: Cada componente tiene una responsabilidad única
3. **Demostrabilidad**: Arquitectura visible y explicable en tiempo real
4. **Escalabilidad Educativa**: Fácil de extender para ejercicios adicionales

---

## Principios Arquitectónicos

### 1. Separation of Concerns (SoC)

**¿Qué es?**
Dividir el sistema en secciones distintas donde cada sección aborda una preocupación específica.

**Implementación en Poll-it:**

```
┌─────────────────────────────────────────────────────────────┐
│                     CONCERNS SEPARADOS                       │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  📦 SHARED (Dominio)                                         │
│  └─ Concern: Definición de Entidades de Negocio             │
│     • PainPoint.cs → Qué ES un dolor                        │
│                                                               │
│  🖥️ SERVER (Backend)                                         │
│  └─ Concern: Lógica de Negocio, Persistencia, Comunicación  │
│     • Data/PainPointDbContext.cs → Cómo GUARDAR             │
│     • Hubs/PainHub.cs → Cómo NOTIFICAR                      │
│     • Program.cs → Cómo EXPONER APIs                        │
│                                                               │
│  🌐 CLIENT (Frontend)                                        │
│  └─ Concern: Presentación, UX, Interacción                  │
│     • Pages/SubmitPain.razor → Cómo CAPTURAR input          │
│     • Pages/PainWall.razor → Cómo MOSTRAR resultados        │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

**Beneficio Educativo:**
Permite explicar "¿Qué pasaría si mezcláramos todo en un archivo?" → Caos, acoplamiento, imposible de mantener.

---

### 2. Single Responsibility Principle (SRP)

**Cada clase/componente hace UNA cosa y la hace bien.**

#### Ejemplo: PainPoint.cs

```csharp
/// <summary>
/// Representa un "dolor" técnico reportado por un usuario.
/// RESPONSABILIDAD: Definir la estructura de datos de un dolor.
/// NO ES RESPONSABLE DE: Cómo guardarlo, cómo mostrarlo, cómo transmitirlo.
/// </summary>
public class PainPoint
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Color { get; set; } = "yellow";
}
```

**¿Por qué no añadimos métodos como `Save()` o `Broadcast()`?**
→ Violaría SRP. El modelo es puro: solo datos, sin lógica.

---

### 3. Dependency Inversion Principle (DIP)

**Depende de abstracciones, no de implementaciones concretas.**

#### Ejemplo: Program.cs (Server)

```csharp
// ❌ MAL: Dependencia concreta
var dbContext = new PainPointDbContext();

// ✅ BIEN: Inyección de Dependencias
builder.Services.AddDbContext<PainPointDbContext>(options => 
    options.UseSqlite("Data Source=painpoints.db"));

// Ahora cualquier componente puede pedir PainPointDbContext
// y el framework lo inyecta automáticamente
```

**Beneficio:**
- Testeable: Podemos inyectar un mock de `DbContext` en tests
- Flexible: Cambiar a SQL Server solo requiere cambiar la configuración DI

---

## Componentes del Sistema

### 1. Shared Project (Poll-it.Shared)

**Responsabilidad:** Definir el modelo de dominio compartido entre cliente y servidor.

#### PainPoint.cs

```csharp
namespace Poll_it.Shared;

/// <summary>
/// Modelo de dominio: Representa un "dolor" técnico.
/// </summary>
/// <remarks>
/// Este modelo se comparte entre el cliente (Blazor WASM) y el servidor (API).
/// Esto garantiza que ambos extremos trabajan con la misma estructura de datos,
/// evitando desincronización y errores de tipo.
/// </remarks>
public class PainPoint
{
    /// <summary>
    /// Identificador único del dolor. Generado automáticamente por la base de datos.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Texto del dolor reportado por el usuario (máximo 200 caracteres).
    /// </summary>
    /// <example>"Las APIs no tienen documentación actualizada"</example>
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Marca de tiempo de cuándo se creó el dolor. Asignada por el servidor.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Color del post-it en la visualización (yellow, blue, green, pink, purple).
    /// Asignado aleatoriamente por el servidor para variedad visual.
    /// </summary>
    public string Color { get; set; } = "yellow";
}
```

**Decisión de Diseño:**
- ¿Por qué un proyecto separado y no duplicar el modelo?
  → **DRY (Don't Repeat Yourself)**: Una fuente de verdad evita inconsistencias.

---

### 2. Server Project (Poll-it.Server)

#### A. Data Layer: PainPointDbContext.cs

```csharp
/// <summary>
/// Contexto de Entity Framework Core para gestionar dolores técnicos.
/// </summary>
/// <remarks>
/// PATRÓN: Repository Pattern
/// Abstrae el acceso a datos. El resto de la aplicación no sabe que usamos SQLite.
/// Podríamos cambiar a SQL Server, PostgreSQL, o MongoDB sin afectar la lógica de negocio.
/// </remarks>
public class PainPointDbContext : DbContext
{
    public PainPointDbContext(DbContextOptions<PainPointDbContext> options) 
        : base(options) { }

    /// <summary>
    /// Colección de dolores técnicos en la base de datos.
    /// </summary>
    public DbSet<PainPoint> PainPoints { get; set; }

    /// <summary>
    /// Configuración del modelo mediante Fluent API.
    /// </summary>
    /// <remarks>
    /// Fluent API > Data Annotations para:
    /// - Mayor control y expresividad
    /// - Separación de concerns (modelo puro, configuración aparte)
    /// - Facilitar cambios sin modificar la entidad
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PainPoint>(entity =>
        {
            // Nombre de tabla explícito
            entity.ToTable("PainPoints");

            // Clave primaria
            entity.HasKey(e => e.Id);

            // CreatedAt con valor por defecto en DB (función SQLite)
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("datetime('now')");

            // Texto obligatorio con longitud máxima
            entity.Property(e => e.Text)
                  .IsRequired()
                  .HasMaxLength(200);

            // Color obligatorio
            entity.Property(e => e.Color)
                  .IsRequired()
                  .HasMaxLength(20);

            // Índice en CreatedAt para consultas rápidas ordenadas por fecha
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
```

**Conceptos Clave:**
- **DbContext**: Unidad de trabajo + Repository genérico
- **Fluent API**: Configuración declarativa del modelo de datos
- **Índices**: Optimización de consultas (educativo: hablar de performance)

---

#### B. Real-Time Layer: PainHub.cs

```csharp
/// <summary>
/// Hub de SignalR para comunicación en tiempo real.
/// </summary>
/// <remarks>
/// CONCEPTO: Patrón Publish-Subscribe (Pub-Sub)
/// 
/// Flujo:
/// 1. Clientes se conectan al Hub
/// 2. Servidor publica eventos (BroadcastNewPain)
/// 3. Todos los clientes suscritos reciben el evento automáticamente
/// 
/// Comparación con HTTP tradicional:
/// - HTTP: Cliente PULL (pregunta constantemente "¿hay algo nuevo?")
/// - SignalR: Servidor PUSH (envía cuando hay algo nuevo)
/// 
/// Ventajas de SignalR:
/// - Latencia baja (WebSockets)
/// - Eficiencia (sin polling innecesario)
/// - Escalable (con backplane Redis para múltiples servidores)
/// </remarks>
public class PainHub : Hub
{
    /// <summary>
    /// Difunde un nuevo dolor a TODOS los clientes conectados.
    /// </summary>
    /// <param name="painPoint">El dolor a transmitir.</param>
    /// <remarks>
    /// Clients.All: Broadcast a todos (incluido el emisor original)
    /// Alternativas:
    /// - Clients.Others: Todos excepto el emisor
    /// - Clients.Group("nombre"): Solo un grupo específico
    /// - Clients.User("userId"): Solo un usuario específico
    /// </remarks>
    public async Task BroadcastNewPain(PainPoint painPoint)
    {
        // Evento "ReceiveNewPain" que los clientes escuchan
        await Clients.All.SendAsync("ReceiveNewPain", painPoint);
    }
}
```

**Explicación Educativa:**

**Pregunta en Clase:** "¿Por qué no usar HTTP Polling?"

**Demostración:**

| Enfoque | Peticiones/min (10 usuarios) | Latencia Promedio | Carga Servidor |
|---------|------------------------------|-------------------|----------------|
| **Polling (cada 2s)** | 300 requests | 1000ms (promedio espera) | Alta |
| **SignalR** | 10 connections | <50ms (instantáneo) | Baja |

**Conclusión:** SignalR es tiempo real verdadero, eficiente y escalable.

---

#### C. API Layer: Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE SERVICIOS (DI Container)
// ==========================================

/// <summary>
/// Registro de DbContext con SQLite.
/// </summary>
/// <remarks>
/// PATRÓN: Dependency Injection
/// El framework se encarga de crear y gestionar el ciclo de vida del DbContext.
/// Beneficios:
/// - Testeable (mock del DbContext en tests)
/// - Configurable (cambiar DB sin modificar código)
/// - Thread-safe (scope por petición HTTP)
/// </remarks>
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

/// <summary>
/// Registro de SignalR para tiempo real.
/// </summary>
builder.Services.AddSignalR();

/// <summary>
/// Configuración de CORS para permitir peticiones desde el cliente Blazor.
/// </summary>
/// <remarks>
/// En producción: Restringir orígenes específicos.
/// En desarrollo: AllowAnyOrigin para facilitar pruebas locales.
/// </remarks>
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ==========================================
// CONFIGURACIÓN DE MIDDLEWARE PIPELINE
// ==========================================

/// <summary>
/// Orden del middleware: IMPORTA.
/// </summary>
/// <remarks>
/// Cada petición pasa por esta cadena en orden:
/// 1. CORS → Valida origen
/// 2. Routing → Identifica endpoint
/// 3. Endpoints → Ejecuta lógica
/// 
/// Analogía: Filtros de seguridad en un aeropuerto.
/// </remarks>

app.UseCors();

// Inicialización de base de datos (demo: EnsureCreated en lugar de migraciones)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PainPointDbContext>();
    db.Database.EnsureCreated(); // Crea DB si no existe
}

// ==========================================
// ENDPOINTS DE LA API (RESTful)
// ==========================================

/// <summary>
/// GET /api/painpoints - Obtener todos los dolores.
/// </summary>
/// <remarks>
/// Patrón: Query
/// - Sin efectos secundarios (idempotente)
/// - Cacheable
/// - Ordenado por fecha descendente (más recientes primero)
/// </remarks>
app.MapGet("/api/painpoints", async (PainPointDbContext db) =>
{
    var painPoints = await db.PainPoints
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    return Results.Ok(painPoints);
});

/// <summary>
/// POST /api/painpoints - Crear un nuevo dolor.
/// </summary>
/// <remarks>
/// Patrón: Command
/// - Modifica el estado del sistema
/// - No idempotente (cada POST crea un nuevo registro)
/// 
/// Flujo:
/// 1. Validar request
/// 2. Asignar color aleatorio
/// 3. Guardar en DB
/// 4. Notificar a todos vía SignalR
/// 5. Retornar 201 Created
/// </remarks>
app.MapPost("/api/painpoints", async (
    CreatePainRequest request,
    PainPointDbContext db,
    IHubContext<PainHub> hubContext) =>
{
    // Validación
    if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Length > 200)
    {
        return Results.BadRequest("El texto debe tener entre 1 y 200 caracteres.");
    }

    // Creación de entidad con color aleatorio
    var colors = new[] { "yellow", "blue", "green", "pink", "purple" };
    var painPoint = new PainPoint
    {
        Text = request.Text,
        Color = colors[Random.Shared.Next(colors.Length)]
    };

    // Persistencia
    db.PainPoints.Add(painPoint);
    await db.SaveChangesAsync();

    // Broadcast en tiempo real
    await hubContext.Clients.All.SendAsync("ReceiveNewPain", painPoint);

    // Respuesta HTTP 201 Created con location header
    return Results.Created($"/api/painpoints/{painPoint.Id}", painPoint);
});

// ==========================================
// MAPEO DE SIGNALR HUB
// ==========================================

/// <summary>
/// Endpoint de WebSocket para SignalR.
/// </summary>
/// <remarks>
/// Los clientes se conectan a /painhub y establecen una conexión persistente.
/// </remarks>
app.MapHub<PainHub>("/painhub");

app.Run();

/// <summary>
/// DTO para la creación de dolores.
/// </summary>
public record CreatePainRequest(string Text);
```

**Conceptos Educativos:**

1. **Minimal API vs. Controllers:**
   - Minimal API: Menos ceremonial, ideal para demos
   - Controllers: Más estructura, mejor para proyectos grandes

2. **Inyección de Dependencias en Endpoints:**
   ```csharp
   app.MapPost("/api/painpoints", async (
       CreatePainRequest request,        // Del body (automático)
       PainPointDbContext db,            // Inyectado por DI
       IHubContext<PainHub> hubContext)  // Inyectado por DI
   ```

3. **IHubContext vs. Hub:**
   - Dentro de un Hub: Usamos `Clients` directamente
   - Fuera de un Hub (como en un endpoint): Usamos `IHubContext<T>`

---

### 3. Client Project (Poll-it.Client)

#### A. Submission Component: SubmitPain.razor

```razor
@page "/submit"
@inject HttpClient Http
@inject NavigationManager Navigation

<div class="min-h-screen bg-gradient-to-br from-indigo-50 via-white to-purple-50 flex items-center justify-center p-6">
    <div class="max-w-2xl w-full">
        <!-- HEADER -->
        <div class="text-center mb-8">
            <h1 class="text-4xl md:text-5xl font-bold text-gray-800 mb-3">
                💬 ¿Cuál es tu mayor dolor técnico hoy?
            </h1>
            <p class="text-gray-600 text-lg">
                Comparte tu frustración con el equipo. ¡Aparecerá en tiempo real!
            </p>
        </div>

        <!-- FORM -->
        <div class="bg-white rounded-2xl shadow-xl p-8">
            @if (errorMessage != null)
            {
                <div class="bg-red-50 border-l-4 border-red-500 p-4 mb-6">
                    <p class="text-red-700">⚠️ @errorMessage</p>
                </div>
            }

            @if (showSuccessMessage)
            {
                <div class="bg-green-50 border-l-4 border-green-500 p-4 mb-6">
                    <p class="text-green-700">✅ ¡Tu dolor ha sido enviado!</p>
                </div>
            }

            <div class="mb-6">
                <label class="block text-gray-700 font-semibold mb-2">
                    Escribe tu dolor (máx. 200 caracteres):
                </label>
                <textarea
                    class="w-full p-4 border-2 border-gray-300 rounded-lg focus:border-indigo-500 focus:ring-2 focus:ring-indigo-200 transition-all resize-none"
                    rows="5"
                    placeholder="Ej: Las APIs no están documentadas..."
                    @bind="painText"
                    @bind:event="oninput"
                    maxlength="200"
                    disabled="@isSubmitting">
                </textarea>
                <div class="text-right text-sm text-gray-500 mt-1">
                    @painText.Length / 200
                </div>
            </div>

            <button
                class="btn-primary-gradient w-full"
                @onclick="SubmitPainPoint"
                disabled="@isSubmitting">
                @if (isSubmitting)
                {
                    <span>⏳ Enviando...</span>
                }
                else
                {
                    <span>🚀 Enviar mi Dolor</span>
                }
            </button>

            <div class="mt-6 text-center">
                <button
                    class="text-indigo-600 hover:text-indigo-800 font-semibold"
                    @onclick="@(() => Navigation.NavigateTo("/wall"))">
                    👁️ Ver el Muro de Dolores
                </button>
            </div>
        </div>
    </div>
</div>

@code {
    private string painText = string.Empty;
    private bool isSubmitting = false;
    private string? errorMessage = null;
    private bool showSuccessMessage = false;

    /// <summary>
    /// Envía el dolor al servidor mediante HTTP POST.
    /// </summary>
    /// <remarks>
    /// Flujo:
    /// 1. Validación local (texto no vacío)
    /// 2. HTTP POST a /api/painpoints
    /// 3. Feedback visual al usuario
    /// 4. Limpieza de formulario
    /// 
    /// Manejo de errores:
    /// - Validación: Mensaje de error
    /// - 4xx/5xx: Mensaje genérico
    /// - Network: Mensaje de conexión
    /// </remarks>
    private async Task SubmitPainPoint()
    {
        errorMessage = null;
        showSuccessMessage = false;

        // Validación local
        if (string.IsNullOrWhiteSpace(painText))
        {
            errorMessage = "Por favor, escribe tu dolor antes de enviar.";
            return;
        }

        isSubmitting = true;

        try
        {
            // HTTP POST
            var request = new { Text = painText };
            var response = await Http.PostAsJsonAsync("https://localhost:7001/api/painpoints", request);

            if (response.IsSuccessStatusCode)
            {
                // Éxito: Feedback visual
                showSuccessMessage = true;
                painText = string.Empty;

                // Auto-dismiss después de 3 segundos
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    showSuccessMessage = false;
                    StateHasChanged();
                });
            }
            else
            {
                errorMessage = "Error al enviar el dolor. Inténtalo nuevamente.";
            }
        }
        catch (Exception)
        {
            errorMessage = "No se pudo conectar con el servidor. Verifica tu conexión.";
        }
        finally
        {
            isSubmitting = false;
        }
    }
}
```

**Conceptos Clave:**

1. **Two-Way Data Binding:**
   ```razor
   @bind="painText"
   @bind:event="oninput"
   ```
   - Sincronización automática entre UI y variable C#
   - `oninput`: Actualización en tiempo real (vs. `onchange` que espera blur)

2. **Validación en Múltiples Capas:**
   - **Cliente (aquí):** UX inmediata, sin roundtrip al servidor
   - **Servidor (API):** Seguridad, no confiar en el cliente
   - **Base de Datos (Constraints):** Última línea de defensa

3. **Manejo de Errores:**
   - Try-catch para excepciones de red
   - Verificación de `response.IsSuccessStatusCode`
   - Mensajes de error claros para el usuario

---

#### B. Display Component: PainWall.razor

```razor
@page "/wall"
@using Microsoft.AspNetCore.SignalR.Client
@inject HttpClient Http
@inject NavigationManager Navigation
@implements IAsyncDisposable

<div class="min-h-screen bg-gradient-to-br from-purple-50 via-white to-indigo-50 p-6">
    <!-- HEADER -->
    <div class="max-w-7xl mx-auto mb-8">
        <div class="text-center mb-6">
            <h1 class="text-5xl font-bold text-gray-800 mb-2">
                🗨️ Muro de Dolores Técnicos
            </h1>
            <p class="text-gray-600 text-lg">
                Observa los dolores aparecer en <span class="font-bold text-indigo-600">tiempo real</span>
            </p>
        </div>

        <!-- STATUS BADGE -->
        <div class="flex justify-center items-center gap-3 mb-6">
            <div class="status-badge @(isConnected ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700")">
                <div class="w-2 h-2 rounded-full @(isConnected ? "bg-green-500 animate-pulse" : "bg-red-500")"></div>
                @(isConnected ? "Conectado" : "Desconectado")
            </div>
            <button
                class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition"
                @onclick="@(() => Navigation.NavigateTo("/submit"))">
                ➕ Añadir Dolor
            </button>
        </div>
    </div>

    <!-- PAIN POINTS MASONRY GRID -->
    @if (isLoading)
    {
        <div class="text-center py-20">
            <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-indigo-600 border-t-transparent"></div>
            <p class="mt-4 text-gray-600">Cargando dolores...</p>
        </div>
    }
    else if (painPoints.Count == 0)
    {
        <div class="text-center py-20">
            <p class="text-2xl text-gray-500">😊 ¡No hay dolores aún! Todo está perfecto.</p>
        </div>
    }
    else
    {
        <div class="max-w-7xl mx-auto columns-1 md:columns-2 lg:columns-3 xl:columns-4 gap-4">
            @foreach (var pain in painPoints)
            {
                var isNew = (DateTime.UtcNow - pain.CreatedAt).TotalSeconds < 10;
                <div class="post-it-card @GetCardClass(pain.Color) @GetBorderClass(pain.Color) mb-4 break-inside-avoid">
                    @if (isNew)
                    {
                        <span class="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold px-2 py-1 rounded-full animate-bounce">
                            ¡NUEVO!
                        </span>
                    }
                    <p class="text-gray-800 font-medium leading-relaxed">
                        @pain.Text
                    </p>
                    <div class="mt-3 text-xs text-gray-600">
                        🕐 @pain.CreatedAt.ToLocalTime().ToString("HH:mm:ss")
                    </div>
                </div>
            }
        </div>
    }
</div>

@code {
    private List<PainPoint> painPoints = new();
    private HubConnection? hubConnection;
    private bool isLoading = true;
    private bool isConnected = false;

    /// <summary>
    /// Inicialización del componente.
    /// </summary>
    /// <remarks>
    /// Flujo de inicialización:
    /// 1. Cargar dolores existentes vía HTTP GET (estado inicial)
    /// 2. Establecer conexión SignalR (para actualizaciones)
    /// 3. Suscribirse a evento "ReceiveNewPain"
    /// 4. Iniciar conexión WebSocket
    /// 
    /// Orden importante: GET antes de SignalR para evitar race conditions
    /// (un dolor nuevo podría llegar antes de cargar el estado inicial).
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        // 1. Carga inicial de datos
        try
        {
            painPoints = await Http.GetFromJsonAsync<List<PainPoint>>("https://localhost:7001/api/painpoints") 
                         ?? new List<PainPoint>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar dolores: {ex.Message}");
            painPoints = new List<PainPoint>();
        }

        // 2. Configuración de SignalR
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/painhub")
            .WithAutomaticReconnect() // Reconexión automática si se pierde conexión
            .Build();

        // 3. Suscripción a eventos de SignalR
        /// <summary>
        /// Callback invocado cuando el servidor emite "ReceiveNewPain".
        /// </summary>
        /// <remarks>
        /// Patrón: Observer
        /// - El cliente observa eventos del servidor
        /// - Cuando llega un evento, ejecuta este callback
        /// - InvokeAsync + StateHasChanged: Actualiza la UI en el thread de Blazor
        /// </remarks>
        hubConnection.On<PainPoint>("ReceiveNewPain", async (newPain) =>
        {
            await InvokeAsync(() =>
            {
                // Insertar al principio (más reciente primero)
                painPoints.Insert(0, newPain);
                StateHasChanged(); // Forzar re-render de Blazor
            });
        });

        // 4. Iniciar conexión
        try
        {
            await hubConnection.StartAsync();
            isConnected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al conectar SignalR: {ex.Message}");
            isConnected = false;
        }

        isLoading = false;
    }

    /// <summary>
    /// Mapea el color de la entidad a una clase CSS de Tailwind.
    /// </summary>
    private string GetCardClass(string color) => color switch
    {
        "yellow" => "bg-yellow-100",
        "blue" => "bg-blue-100",
        "green" => "bg-green-100",
        "pink" => "bg-pink-100",
        "purple" => "bg-purple-100",
        _ => "bg-gray-100"
    };

    /// <summary>
    /// Mapea el color de la entidad a una clase de borde CSS.
    /// </summary>
    private string GetBorderClass(string color) => color switch
    {
        "yellow" => "border-yellow-300",
        "blue" => "border-blue-300",
        "green" => "border-green-300",
        "pink" => "border-pink-300",
        "purple" => "border-purple-300",
        _ => "border-gray-300"
    };

    /// <summary>
    /// Limpieza de recursos al destruir el componente.
    /// </summary>
    /// <remarks>
    /// Implementación de IAsyncDisposable.
    /// Importante: Cerrar la conex
