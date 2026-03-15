# 📚 Poll-it - Documentación Técnica Completa

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Requisitos del Sistema](#requisitos-del-sistema)
3. [Instalación y Configuración](#instalación-y-configuración)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Componentes Principales](#componentes-principales)
6. [Flujos de Trabajo](#flujos-de-trabajo)
7. [API RESTful](#api-restful)
8. [Comunicación en Tiempo Real (SignalR)](#comunicación-en-tiempo-real-signalr)
9. [Base de Datos](#base-de-datos)
10. [Guía de Desarrollo](#guía-de-desarrollo)
11. [Troubleshooting](#troubleshooting)

---

## Descripción General

**Poll-it** es una aplicación web moderna diseñada para capturar y visualizar "dolores" técnicos (frustraciones técnicas) en tiempo real. Fue construida como herramienta educativa para demostrar arquitectura de software moderna con .NET 10.

### Características Principales

- ✅ **Captura en Tiempo Real**: Los usuarios envían sus frustaciones técnicas instantáneamente
- ✅ **Visualización Colaborativa**: Todos los usuarios ven los nuevos puntos de dolor en vivo
- ✅ **Interfaz Moderna**: Diseño responsive con Tailwind CSS
- ✅ **Arquitectura Educativa**: Código limpio, bien documentado y escalable
- ✅ **Separación de Concerns**: Cliente, servidor y datos completamente desacoplados

### Stack Tecnológico

| Capa | Tecnología | Versión |
|------|-----------|---------|
| **Frontend** | Blazor WebAssembly | .NET 10 |
| **Backend** | ASP.NET Core Minimal API | .NET 10 |
| **Base de Datos** | SQLite | Última |
| **ORM** | Entity Framework Core | 10.x |
| **Comunicación RT** | SignalR | Última |
| **Styling** | Tailwind CSS | Última |
| **Protocolo** | HTTP/REST + WebSocket | Estándar |

---

## Requisitos del Sistema

### Mínimo Recomendado

- **Sistema Operativo**: Windows 10+, macOS 11+, Linux (Ubuntu 20.04+)
- **.NET Runtime**: .NET 10.0 SDK o superior
- **Navegador**: Chrome/Edge/Firefox/Safari (versiones recientes)
- **Memoria RAM**: 2 GB mínimo, 4 GB recomendado
- **Espacio Disco**: 500 MB libres

### Herramientas Necesarias

```bash
# Verificar .NET instalado
dotnet --version

# Debe mostrar: .NET 10.0.x o superior
```

---

## Instalación y Configuración

### Paso 1: Clonar el Repositorio

```bash
git clone <repositorio-url>
cd Poll_it
```

### Paso 2: Restaurar Dependencias

```bash
# En la raíz del proyecto (sln)
dotnet restore
```

### Paso 3: Construir la Solución

```bash
# Construir todos los proyectos
dotnet build
```

### Paso 4: Ejecutar la Aplicación

#### Opción A: Ejecutar ambos (Server + Client)

```bash
# Terminal 1: Ejecutar el servidor
cd Poll-it.Server
dotnet run

# Terminal 2: Ejecutar el cliente
cd Poll-it.Client
dotnet run
```

#### Opción B: Ejecutar Server solamente

```bash
cd Poll-it.Server
dotnet run
# El servidor estará en: https://localhost:7001
```

El cliente Blazor WASM se cargará automáticamente y se conectará al servidor.

### Acceso a la Aplicación

- **URL Principal**: https://localhost:3000 (o puerto asignado)
- **API Backend**: https://localhost:7001
- **Páginas Disponibles**:
  - `/` → Home (redirecciona a /submit)
  - `/submit` → Formulario para enviar dolores
  - `/wall` → Panel de visualización en tiempo real

---

## Estructura del Proyecto

```
Poll-it/
│
├── 📦 Poll-it.Shared/
│   ├── PainPoint.cs                 # Modelo de dominio compartido
│   └── Poll-it.Shared.csproj
│
├── 🖥️ Poll-it.Server/
│   ├── Data/
│   │   └── PainPointDbContext.cs    # Contexto EF Core
│   ├── Hubs/
│   │   └── PainHub.cs               # Hub de SignalR
│   ├── Properties/
│   │   └── launchSettings.json      # Configuración de puertos
│   ├── Program.cs                   # Punto de entrada, Minimal API
│   ├── appsettings.json             # Configuración de producción
│   ├── appsettings.Development.json # Configuración de desarrollo
│   ├── Poll-it.Server.csproj
│   └── Poll-it.Server.http          # Archivo para probar endpoints
│
├── 🌐 Poll-it.Client/
│   ├── Pages/
│   │   ├── Home.razor               # Página de inicio
│   │   ├── SubmitPain.razor         # Formulario de entrada
│   │   ├── PainWall.razor           # Panel en tiempo real
│   │   └── NotFound.razor           # Página 404
│   ├── Layout/
│   │   ├── MainLayout.razor         # Layout principal
│   │   ├── MainLayout.razor.css     # Estilos del layout
│   │   ├── NavMenu.razor            # Menú de navegación
│   │   └── NavMenu.razor.css        # Estilos del menú
│   ├── wwwroot/
│   │   ├── index.html               # HTML root
│   │   ├── css/
│   │   │   └── app.css              # Estilos Tailwind
│   │   └── lib/
│   │       └── bootstrap/           # Bootstrap (opcional)
│   ├── _Imports.razor               # Imports globales
│   ├── App.razor                    # Componente raíz y router
│   ├── Program.cs                   # Configuración del cliente
│   └── Poll-it.Client.csproj
│
├── 📄 ARCHITECTURE.md               # Documentación de arquitectura
├── 📄 README.md                     # Descripción general
├── 📄 DOCUMENTACION-TECNICA.md      # Este archivo
├── 📄 Poll-it.slnx                  # Archivo de solución
└── 📄 Various .md files

```

### Archivos Clave

| Archivo | Propósito | Responsabilidad |
|---------|----------|-----------------|
| `PainPoint.cs` | Modelo de datos | Definir estructura de un "dolor" |
| `PainPointDbContext.cs` | Acceso a datos | Mapear C# a SQLite |
| `PainHub.cs` | Comunicación RT | Broadcast sobre WebSocket |
| `Program.cs (Server)` | Configuración API | Endpoints, middlewares, servicios |
| `Program.cs (Client)` | Configuración WASM | Cliente HTTP, rutas |
| `SubmitPain.razor` | Formulario | Capturar entrada del usuario |
| `PainWall.razor` | Visualización | Mostrar dolores en tiempo real |

---

## Componentes Principales

### 1️⃣ Poll-it.Shared: Modelo de Dominio

#### PainPoint.cs

```csharp
public class PainPoint
{
    public int Id { get; set; }                    // ID único
    public string Text { get; set; }               // Texto del dolor (max 500 chars)
    public DateTime CreatedAt { get; set; }        // Timestamp de creación
    public string Color { get; set; }              // Color para visualización
}
```

**Responsabilidades:**
- Definir la estructura de datos
- Ser compartido entre cliente y servidor
- NO contiene lógica de negocio

---

### 2️⃣ Poll-it.Server: Backend API

#### A) PainPointDbContext.cs - Capa de Datos

```csharp
public class PainPointDbContext : DbContext
{
    public DbSet<PainPoint> PainPoints => Set<PainPoint>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar tabla, índices, restricciones
        modelBuilder.Entity<PainPoint>(entity =>
        {
            entity.ToTable("PainPoints");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Text).IsRequired().HasMaxLength(500);
        });
    }
}
```

**Responsabilidades:**
- Mapear objetos C# a tablas SQLite
- Gestionar migraciones de BD
- Encapsular consultas
- Aplicar validaciones a nivel de BD

**Características:**
- ✅ Auto-incremento de ID
- ✅ Validación de longitud de texto (500 caracteres máx)
- ✅ Timestamps automáticos
- ✅ Soporte para índices

---

#### B) PainHub.cs - Comunicación en tiempo real

```csharp
public class PainHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Se ejecuta cuando un cliente se conecta
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Se ejecuta cuando un cliente se desconecta
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastNewPain(PainPoint painPoint)
    {
        // Enviar el nuevo dolor a TODOS los clientes conectados
        await Clients.All.SendAsync("ReceiveNewPain", painPoint);
    }
}
```

**Responsabilidades:**
- Gestionar conexiones WebSocket
- Broadcast de eventos a clientes
- Manejar desconexiones

**Patrón de Comunicación:**

```
Cliente 1                           Servidor                           Cliente 2
    │                                   │                                  │
    ├─── Envía nuevo PainPoint ────────►│                                  │
    │                                   ├─ Guarda en BD                    │
    │                                   │                                  │
    │                                   ├─ Llama a PainHub.Broadcast      │
    │                                   │                                  │
    │             ◄─────────────────── Notifica ────────────────────────►   │
    │        "ReceiveNewPain"           │        "ReceiveNewPain"          │
    │                                   │                                  │
```

---

#### C) Program.cs - Configuración de la API

```csharp
// SERVICIOS (Dependency Injection)
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", 
    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// MIDDLEWARE
app.UseCors("AllowAll");
app.MapHub<PainHub>("/painhub");  // Accesible en ws://localhost:7001/painhub

// ENDPOINTS (Minimal API)
app.MapPost("/api/painpoints", SubmitPainHandler);
app.MapGet("/api/painpoints", GetAllPainPointsHandler);
app.MapDelete("/api/painpoints/{id}", DeletePainPointHandler);

app.Run();
```

---

### 3️⃣ Poll-it.Client: Frontend Blazor

#### A) App.razor - Raíz de la Aplicación

```razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Page not found</p>
        </LayoutView>
    </NotFound>
</Router>
```

**Responsabilidades:**
- Renderizar la aplicación
- Manejar enrutamiento
- Proporcionar layout por defecto

---

#### B) SubmitPain.razor - Formulario de Entrada

```razor
@page "/submit"
@using Poll_it.Shared
@using Microsoft.AspNetCore.SignalR.Client
@inject HttpClient Http

<textarea @bind="painText" />
<button @onclick="SubmitPainPoint">Enviar</button>
```

**Flujo:**
1. Usuario ingresa texto
2. Hace click en "Enviar"
3. Se valida que no esté vacío
4. Se envía POST a `/api/painpoints`
5. Se muestra confirmación/error

**Responsabilidades:**
- Capturar entrada del usuario
- Validación básica
- Envío HTTP al servidor
- Feedback visual

---

#### C) PainWall.razor - Panel en Tiempo Real

```razor
@page "/wall"
@using Poll_it.Shared
@using Microsoft.AspNetCore.SignalR.Client
@implements IAsyncDisposable

<div class="pain-wall">
    @foreach (var pain in pains)
    {
        <div class="post-it" style="background-color: @pain.Color">
            @pain.Text
        </div>
    }
</div>

@code {
    private List<PainPoint> pains = [];
    private HubConnection? hubConnection;

    protected override async Task OnInitializedAsync()
    {
        // Cargar dolores existentes
        pains = await Http.GetFromJsonAsync<List<PainPoint>>("/api/painpoints");

        // Conectar a SignalR
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/painhub")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<PainPoint>("ReceiveNewPain", pain =>
        {
            pains.Add(pain);
            StateHasChanged();
        });

        await hubConnection.StartAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (hubConnection is not null)
            await hubConnection.DisposeAsync();
    }
}
```

**Responsabilidades:**
- Cargar dolores iniciales
- Conectar a SignalR
- Escuchar eventos de nuevos dolores
- Actualizar UI en tiempo real

---

## Flujos de Trabajo

### Flujo 1: Envío de un Nuevo Dolor

```
┌─────────────────────────────────────────────────────────────────────┐
│                    FLUJO DE ENVÍO DE DOLOR                          │
└─────────────────────────────────────────────────────────────────────┘

1. CAPTURA (Cliente - SubmitPain.razor)
   └─► Usuario abre formulario
   └─► Ingresa texto (máx 200 caracteres)
   └─► Click "Enviar Dolor"

2. VALIDACIÓN (Cliente)
   └─► ¿Texto no vacío? ✓
   └─► ¿Texto ≤ 200 chars? ✓
   └─► Mostrar loading

3. TRANSMISIÓN (HTTP)
   └─► POST https://localhost:7001/api/painpoints
   └─► Body: { "text": "..." }
   └─► Content-Type: application/json

4. PERSISTENCIA (Servidor - Program.cs)
   └─► Validar texto nuevamente
   └─► Asignar color aleatorio
   └─► Asignar timestamp
   └─► Guardar en SQLite vía EF Core

5. BROADCAST (Servidor - PainHub)
   └─► Query: "SELECT * FROM PainPoints WHERE Id = último"
   └─► hubConnection.Clients.All.SendAsync("ReceiveNewPain", painPoint)
   └─► Transmitir a TODOS los clientes conectados

6. ACTUALIZACIÓN (Clientes - PainWall.razor)
   └─► Recibir evento "ReceiveNewPain"
   └─► pains.Add(newPain)
   └─► StateHasChanged() → Re-render
   └─► Post-it aparece en muro

7. CONFIRMACIÓN (Cliente - SubmitPain.razor)
   └─► Mostrar "✓ Dolor enviado"
   └─► Limpiar formulario
   └─► Ocultar loading

Tiempo Total: ~100-300ms
```

### Flujo 2: Visualización en Tiempo Real

```
┌─────────────────────────────────────────────────────────────────────┐
│              FLUJO DE VISUALIZACIÓN EN TIEMPO REAL                  │
└─────────────────────────────────────────────────────────────────────┘

1. USUARIO ABRE /wall
   └─► PainWall.razor: OnInitializedAsync()

2. CARGAR DATOS INICIALES
   └─► GET /api/painpoints
   └─► Traer todos los dolores existentes
   └─► pains = [PainPoint, PainPoint, ...]

3. CONECTAR SIGNALR
   └─► new HubConnectionBuilder()
   └─► .WithUrl("wss://localhost:7001/painhub")
   └─► .Build()
   └─► .StartAsync()

4. REGISTRAR LISTENER
   └─► hubConnection.On<PainPoint>("ReceiveNewPain", ...)
   └─► Cuando otro usuario envía: se dispara callback

5. ESCUCHAR EVENTOS
   └─► Cliente 1 envía nuevo dolor
   └─► Servidor broadcast a todos
   └─► Cliente 2, 3, 4... reciben evento
   └─► Se actualiza lista y se re-renderiza

6. ACTUALIZACIÓN VISUAL INSTANTÁNEA
   └─► Nuevo Post-it aparece sin refrescar página
   └─► Animación suave
   └─► UI reactiva

Latencia: ~50-100ms en LAN, ~200-500ms en internet
```

---

## API RESTful

### Endpoint: POST /api/painpoints

**Descripción:** Crear un nuevo punto de dolor

**Request:**
```http
POST /api/painpoints HTTP/1.1
Content-Type: application/json

{
    "text": "Los tests unitarios no paran de fallar"
}
```

**Response (201 Created):**
```json
{
    "id": 5,
    "text": "Los tests unitarios no paran de fallar",
    "createdAt": "2025-03-05T14:23:45.123Z",
    "color": "yellow"
}
```

**Códigos de Respuesta:**
- `201 Created` - Éxito
- `400 Bad Request` - Texto vacío o inválido
- `500 Internal Server Error` - Error del servidor

---

### Endpoint: GET /api/painpoints

**Descripción:** Obtener todos los puntos de dolor

**Request:**
```http
GET /api/painpoints HTTP/1.1
```

**Response (200 OK):**
```json
[
    {
        "id": 1,
        "text": "Bug en producción",
        "createdAt": "2025-03-05T14:15:00Z",
        "color": "red"
    },
    {
        "id": 2,
        "text": "Falta documentación",
        "createdAt": "2025-03-05T14:16:30Z",
        "color": "blue"
    }
]
```

**Códigos de Respuesta:**
- `200 OK` - Éxito (lista vacía si no hay dolores)
- `500 Internal Server Error` - Error del servidor

---

### Endpoint: DELETE /api/painpoints/{id}

**Descripción:** Eliminar un punto de dolor

**Request:**
```http
DELETE /api/painpoints/5 HTTP/1.1
```

**Response (200 OK):**
```json
{
    "message": "Punto de dolor eliminado exitosamente"
}
```

**Códigos de Respuesta:**
- `200 OK` - Eliminado exitosamente
- `404 Not Found` - El ID no existe
- `500 Internal Server Error` - Error del servidor

---

## Comunicación en Tiempo Real (SignalR)

### Arquitectura SignalR

```
┌────────────────────────────────────────────────────────────────────┐
│                      SIGNALR - COMUNICACIÓN RT                      │
└────────────────────────────────────────────────────────────────────┘

CONEXIÓN (WebSocket)
  Client                                    Server
    │                    Connect               │
    ├──────────────────────────────────────────►
    │                 ConnectionId=ABC123      │
    │◄──────────────────────────────────────────
    │
    │      Hub.On("ReceiveNewPain", callback)
    ├─ Registrar listener
    │
    │                   Broadcast             │
    │◄──────────────────────────────────────────
    │      { id: 5, text: "...", ... }        │
    │
    │              StateHasChanged()
    └─ Re-render UI
```

### Métodos del Hub

#### Server → Clients (SendAsync)

```csharp
// Enviar a TODOS los clientes
await Clients.All.SendAsync("ReceiveNewPain", painPoint);

// Enviar a cliente específico
await Clients.Client(connectionId).SendAsync("ReceiveNewPain", painPoint);

// Enviar a grupo de clientes
await Clients.Group("sala1").SendAsync("ReceiveNewPain", painPoint);
```

#### Client → Server (InvokeAsync)

```csharp
// Llamar método del servidor
await hubConnection.InvokeAsync("BroadcastNewPain", painPoint);
```

### Ciclo de Vida de Conexión

```csharp
hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7001/painhub")
    .WithAutomaticReconnect()    // Reconexión automática
    .Build();

// Evento: cliente conectado
hubConnection.On<PainPoint>("ReceiveNewPain", (pain) => {
    pains.Add(pain);
    StateHasChanged();
});

// Conectar
await hubConnection.StartAsync();

// ... usar ...

// Desconectar
await hubConnection.DisposeAsync();
```

---

## Base de Datos

### Schema (Tabla PainPoints)

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-----------|
| `Id` | INTEGER | PRIMARY KEY, AUTO_INCREMENT | ID único |
| `Text` | TEXT | NOT NULL, MAX 500 | Contenido del dolor |
| `CreatedAt` | DATETIME | NOT NULL, DEFAULT=NOW | Timestamp de creación |
| `Color` | TEXT | NOT NULL, DEFAULT='yellow' | Color del Post-it |

### SQL Equivalente

```sql
CREATE TABLE PainPoints (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Text TEXT NOT NULL CHECK(length(Text) <= 500),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Color TEXT NOT NULL DEFAULT 'yellow'
);
```

### Ubicación de la BD

- **Archivo**: `painpoints.db`
- **Ubicación**: Raíz del directorio del servidor (donde se ejecuta)
- **Tamaño típico**: < 1 MB
- **Tipo**: SQLite (archivo único, sin servidor)

### Comandos Útiles

```bash
# Ver la BD con SQLite CLI (si está instalado)
sqlite3 painpoints.db

# Consultar todos los dolores
SELECT * FROM PainPoints ORDER BY CreatedAt DESC;

# Contar dolores
SELECT COUNT(*) FROM PainPoints;

# Limpiar tabla
DELETE FROM PainPoints;
```

### Migraciones

Las migraciones se manejan a través de Entity Framework Core:

```bash
# Crear migración (si agregas propiedades a PainPoint)
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update LastGoodMigration
```

---

## Guía de Desarrollo

### Configuración del Entorno

#### 1. Instalar Requisitos

```bash
# Verificar .NET
dotnet --version

# Instalar .NET 10 (si no lo tiene)
# Descargar de: https://dotnet.microsoft.com/download/dotnet/10.0
```

#### 2. Clonar y Restaurar

```bash
git clone <repo>
cd Poll_it
dotnet restore
```

#### 3. Inicializar la BD

```bash
cd Poll-it.Server
dotnet ef database update
cd ..
```

---

### Estructura de Carpetas Recomendada para Extensiones

```
Poll-it.Server/
├── Data/                          # Acceso a datos
│   └── PainPointDbContext.cs
├── Hubs/                          # Comunicación tiempo real
│   └── PainHub.cs
├── Handlers/                      # 🆕 Lógica de endpoints
│   ├── PainPointHandlers.cs
│   └── ValidationHandlers.cs
├── Models/                        # 🆕 DTOs y respuestas
│   ├── CreatePainRequest.cs
│   └── ApiResponse.cs
├── Services/                      # 🆕 Lógica de negocio
│   └── PainPointService.cs
├── Middleware/                    # 🆕 Middleware personalizado
│   └── ErrorHandlingMiddleware.cs
└── Program.cs
```

---

### Agregar Nueva Funcionalidad: Ejemplo

**Requisito:** Obtener solo los dolores de las últimas 24 horas

#### Paso 1: Crear Handler

```csharp
// Poll-it.Server/Handlers/PainPointHandlers.cs
app.MapGet("/api/painpoints/recent", async (PainPointDbContext db) =>
{
    var yesterday = DateTime.UtcNow.AddHours(-24);
    var recent = await db.PainPoints
        .Where(p => p.CreatedAt > yesterday)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();
    
    return Results.Ok(recent);
})
.WithName("GetRecentPainPoints")
.WithOpenApi();
```

#### Paso 2: Probar con REST Client (en Poll-it.Server.http)

```http
GET https://localhost:7001/api/painpoints/recent HTTP/1.1
```

#### Paso 3: Usar en Cliente (PainWall.razor)

```csharp
// En OnInitializedAsync
var recentPains = await Http.GetFromJsonAsync<List<PainPoint>>(
    "/api/painpoints/recent");
```

---

### Debugging y Logging

#### Habilitar Logging Detallado

```csharp
// En Program.cs (Server)
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started");
```

#### Debugging en VS Code

```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Server",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/Poll-it.Server/bin/Debug/net10.0/Poll-it.Server.dll",
            "args": [],
            "cwd": "${workspaceFolder}/Poll-it.Server",
            "preLaunchTask": "build"
        }
    ]
}
```

---

### Testing

#### Probar Endpoints con Rest File

```http
### Crear dolor
POST https://localhost:7001/api/painpoints HTTP/1.1
Content-Type: application/json

{
    "text": "Problema de sincronización en BD"
}

### Obtener todos
GET https://localhost:7001/api/painpoints HTTP/1.1

### Eliminar
DELETE https://localhost:7001/api/painpoints/1 HTTP/1.1
```

---

## Troubleshooting

### ❌ "Connection refused" al conectar a servidor

**Causa:** El servidor no está ejecutándose

**Solución:**
```bash
cd Poll-it.Server
dotnet run
# Debe mostrar: "Application started. Press Ctrl+C to shut down."
```

---

### ❌ "CORS policy blocked"

**Causa:** Servidor rechaza solicitudes del cliente

**Solución:** Verificar CORS en `Program.cs`:

```csharp
builder.Services.AddCors(options => 
    options.AddPolicy("AllowWasm", policy =>
        policy.AllowAnyOrigin()   // ← Permitir cualquier origen
              .AllowAnyMethod()
              .AllowAnyHeader()));

app.UseCors("AllowWasm");  // ← Aplicar ANTES de endpoints
```

---

### ❌ "SignalR connection failed"

**Causa:** Cliente no puede conectar a Hub

**Solución:**
```csharp
// Verificar URL correcta
hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7001/painhub")  // ← Ajustar puerto
    .WithAutomaticReconnect()
    .Build();
```

---

### ❌ "Database locked" error

**Causa:** Múltiples accesos simultáneos a SQLite

**Solución:** SQLite tiene limitaciones en concurrencia. Para producción, usar SQL Server:

```csharp
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlServer("Server=.;Database=PollIt;Integrated Security=true;"));
```

---

### ❌ "Port already in use"

**Causa:** Otro proceso usa el puerto

**Solución:**
```bash
# Cambiar puerto en launchSettings.json
# O matar proceso:
# Windows:
netstat -ano | findstr :7001
taskkill /PID <PID> /F

# Linux/Mac:
lsof -ti :7001 | xargs kill -9
```

---

### ❌ Base de datos corrupta

**Causa:** Archivo `painpoints.db` dañado

**Solución:**
```bash
# Eliminar y recrear
rm painpoints.db
dotnet ef database update
```

---

## Comandos Útiles

```bash
# Limpiar solución
dotnet clean

# Reconstruir
dotnet build

# Ejecutar servidor
cd Poll-it.Server
dotnet run

# Ejecutar cliente (otra terminal)
cd Poll-it.Client
dotnet run

# Ver logs verbosos
dotnet run --verbosity=diagnostic

# Publicar para producción
dotnet publish -c Release -o ./publish

# Ver versión de .NET
dotnet --version
```

---

## Recursos Adicionales

- 📚 [Microsoft .NET Documentation](https://docs.microsoft.com/dotnet)
- 📚 [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- 📚 [Entity Framework Core](https://docs.microsoft.com/ef/core)
- 📚 [SignalR Documentation](https://docs.microsoft.com/aspnet/core/signalr)
- 📚 [ASP.NET Core](https://docs.microsoft.com/aspnet/core)

---

## Glosario

| Término | Definición |
|---------|-----------|
| **WASM** | WebAssembly - Código compilado que corre en navegadores |
| **SignalR** | Biblioteca para comunicación en tiempo real |
| **Hub** | Centro de comunicación en SignalR |
| **DbContext** | Clase que representa sesión con BD en EF Core |
| **Minimal API** | Forma simplificada de crear APIs en .NET |
| **CORS** | Cross-Origin Resource Sharing - Política de seguridad |
| **DTO** | Data Transfer Object - Objeto para transferir datos |
| **JWT** | JSON Web Token - Autenticación segura |

---

## Información de Contacto / Soporte

- 📧 Developer: [Tu email]
- 🔗 Repository: [Tu repositorio]
- 📋 Issues: [URL de issues]

---

**Última actualización**: Marzo 5, 2025  
**Versión de documento**: 1.0  
**Estado**: ✅ Producción
