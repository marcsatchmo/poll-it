# 📝 Poll-it - Captura de Dolores Técnicos en Tiempo Real

**Una aplicación educativa para demostrar arquitectura de software moderna con .NET 10 y Blazor WebAssembly**

---

## 🎯 Propósito

Poll-it es una aplicación diseñada específicamente para **clases de arquitectura de software**. Su objetivo es capturar "dolores" (frustraciones técnicas) de analistas funcionales en tiempo real durante una clase, permitiendo visualizar instantáneamente todos los puntos de dolor en una pantalla común.

La aplicación sirve como **ejemplo práctico** para explicar:
- ✅ Separación de Concerns (Separation of Concerns)
- ✅ Arquitectura Cliente-Servidor
- ✅ Comunicación en Tiempo Real con SignalR
- ✅ Patrón Repository con Entity Framework Core
- ✅ API RESTful con ASP.NET Core Minimal API
- ✅ Frontend Moderno con Blazor WebAssembly

---

## 🏗️ Arquitectura del Sistema

### Diagrama de Alto Nivel

```
┌─────────────────────────────────────────────────────────────────┐
│                        POLL-IT SYSTEM                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌─────────────────┐         ┌──────────────────┐               │
│  │  Blazor Client  │ ◄─────► │   ASP.NET Core   │               │
│  │   (Frontend)    │  HTTP   │   Web API        │               │
│  │                 │  REST   │   (Backend)      │               │
│  └────────┬────────┘         └────────┬─────────┘               │
│           │                           │                          │
│           │ SignalR WebSocket         │ Entity Framework Core   │
│           │ (Tiempo Real)             │                          │
│           │                           ▼                          │
│           │                  ┌────────────────┐                  │
│           └────────────────► │  SQLite DB     │                  │
│                              │  painpoints.db │                  │
│                              └────────────────┘                  │
│                                                                   │
└───────────────────────────────────────────────���─���───────────────┘
```

### Flujo de Datos

**1. Usuario envía un dolor (Submit Pain):**
```
[Usuario] → [SubmitPain.razor] 
          → HTTP POST /api/painpoints 
          → [Program.cs API Endpoint] 
          → [EF Core DbContext] 
          → [SQLite Database]
          → [SignalR PainHub.BroadcastNewPain()] 
          → [Todos los Clientes Conectados]
```

**2. Visualización en tiempo real (Pain Wall):**
```
[Usuario] → [PainWall.razor] 
          → HTTP GET /api/painpoints (carga inicial)
          → SignalR Connection al Hub
          → Escucha evento "ReceiveNewPain"
          → Actualiza UI automáticamente
```

---

## 🗂️ Estructura del Proyecto

```
Poll-it/
│
├── Poll-it.Shared/              # 📦 Proyecto compartido
│   ├── PainPoint.cs             # Modelo de dominio (entidad principal)
│   └── Poll-it.Shared.csproj
│
├── Poll-it.Server/              # 🖥️ Backend API
│   ├── Data/
│   │   └── PainPointDbContext.cs   # Contexto de Entity Framework Core
│   ├── Hubs/
│   │   └── PainHub.cs              # SignalR Hub para tiempo real
│   ├── Program.cs                  # Configuración de la API y Middleware
│   ├── appsettings.json
│   └── Poll-it.Server.csproj
│
├── Poll-it.Client/              # 🌐 Frontend Blazor WASM
│   ├── Pages/
│   │   ├── Home.razor              # Redirección inicial
│   │   ├── SubmitPain.razor        # Formulario de envío
│   │   └── PainWall.razor          # Muro de visualización
│   ├── wwwroot/
│   │   ├── css/app.css             # Estilos con Tailwind CSS
│   │   └── index.html              # HTML principal
│   ├── App.razor                   # Router de la aplicación
│   ├── Program.cs                  # Configuración del cliente
│   └── Poll-it.Client.csproj
│
├── Poll-it.slnx                 # Solución de Visual Studio
└── README.md                    # Este archivo
```

---

## 🔧 Tecnologías Utilizadas

### Backend
- **Framework**: .NET 10
- **API**: ASP.NET Core Minimal API
- **Tiempo Real**: SignalR
- **ORM**: Entity Framework Core 10
- **Base de Datos**: SQLite (portable, sin instalación)

### Frontend
- **Framework**: Blazor WebAssembly
- **UI/UX**: Tailwind CSS (vía CDN)
- **SignalR Client**: Microsoft.AspNetCore.SignalR.Client

---

## 📋 Prerrequisitos

- **.NET 10 SDK** (o superior)
  - Descargar: https://dotnet.microsoft.com/download/dotnet/10.0
- **Visual Studio 2022** (opcional, recomendado) o **Visual Studio Code**
- **Navegador web moderno** (Chrome, Edge, Firefox)

---

## 🚀 Cómo Ejecutar la Aplicación

### Opción 1: Línea de Comandos

1. **Clonar o descargar el repositorio**

2. **Restaurar dependencias**
   ```bash
   dotnet restore Poll-it.slnx
   ```

3. **Ejecutar el servidor backend**
   ```bash
   cd Poll-it.Server
   dotnet run --launch-profile https
   ```
   
   El servidor se iniciará en: `https://localhost:7001` (y `http://localhost:5000`)

4. **En una nueva terminal, ejecutar el cliente**
   ```bash
   cd Poll-it.Client
   dotnet run --launch-profile https
   ```
   
   El cliente se iniciará en: `https://localhost:7219` (y `http://localhost:5048`)

5. **Abrir el navegador**
   - El navegador se abrirá automáticamente en: `https://localhost:7219`
   - Si no, navegar manualmente a esa URL

### Opción 2: Visual Studio 2022

1. Abrir el archivo `Poll-it.slnx`
2. Configurar múltiples proyectos de inicio:
   - Click derecho en la solución → **Propiedades**
   - Seleccionar **Proyectos de inicio múltiples**
   - Configurar **Poll-it.Server** y **Poll-it.Client** como "Iniciar"
3. Presionar `F5` o click en **Iniciar**

### Opción 3: Visual Studio Code

1. Instalar la extensión **C# Dev Kit**
2. Abrir la carpeta del proyecto
3. Ejecutar el servidor y cliente en terminales separadas (ver Opción 1)

---

## 🎨 Uso de la Aplicación

### Para Estudiantes (Enviar Dolores)

1. Acceder a la URL del cliente
2. En la página principal, escribir tu "dolor técnico" (máx. 200 caracteres)
3. Hacer click en **"🚀 Enviar mi Dolor"**
4. Tu dolor aparecerá instantáneamente en el muro de todos

### Para el Instructor (Visualizar Muro)

1. Acceder a `/wall` en cualquier navegador
2. Ver todos los dolores en un layout tipo "Post-it" colorido
3. Los nuevos dolores aparecen automáticamente en tiempo real
4. Indicador de conexión muestra el estado de SignalR

### Demostración en Clase

**Escenario recomendado:**
1. Proyectar el "Muro de Dolores" en una pantalla grande
2. Los estudiantes envían dolores desde sus dispositivos
3. Todos ven aparecer las notas instantáneamente
4. Usar esto para explicar:
   - Comunicación en tiempo real
   - Arquitectura cliente-servidor
   - Broadcasting con SignalR
   - Persistencia de datos
   - Separación de concerns

---

## 🏛️ Conceptos de Arquitectura Explicados

### 1. Separación de Concerns (SoC)

**Problema:** Código monolítico donde todo está mezclado es difícil de mantener.

**Solución en Poll-it:**
- **Shared**: Modelos de dominio (PainPoint.cs)
- **Server**: Lógica de negocio, persistencia, comunicación
- **Client**: Presentación, interacción con usuario

Cada capa tiene una **responsabilidad única** y está **aislada**.

### 2. Patrón Repository

**Ubicación:** `PainPointDbContext.cs`

```csharp
public class PainPointDbContext : DbContext
{
    public DbSet<PainPoint> PainPoints { get; set; }
    // Abstrae el acceso a datos
}
```

**Beneficio:** El resto de la aplicación no conoce SQLite. Podríamos cambiar a SQL Server sin afectar el código de negocio.

### 3. API RESTful

**Ubicación:** `Program.cs` (endpoints)

```csharp
// GET - Obtener todos los dolores
app.MapGet("/api/painpoints", async (PainPointDbContext db) => {...});

// POST - Crear nuevo dolor
app.MapPost("/api/painpoints", async (request, db, hub) => {...});
```

**Principios REST:**
- Recursos identificados por URLs
- Operaciones mediante verbos HTTP (GET, POST)
- Stateless (sin estado en el servidor entre peticiones)

### 4. Comunicación en Tiempo Real con SignalR

**Ubicación:** `PainHub.cs` y `PainWall.razor`

**¿Por qué SignalR y no HTTP Polling?**

| Enfoque | Problema |
|---------|----------|
| **Polling** | Cliente pregunta "¿hay algo nuevo?" cada X segundos → Ineficiente |
| **SignalR** | Servidor **empuja** datos cuando ocurren → Eficiente, tiempo real |

**Flujo:**
1. Cliente se conecta al Hub: `hubConnection.StartAsync()`
2. Cliente escucha eventos: `hubConnection.On<PainPoint>("ReceiveNewPain", ...)`
3. Servidor envía a todos: `await Clients.All.SendAsync("ReceiveNewPain", pain)`

### 5. Entity Framework Core - Code First

**Ubicación:** `PainPointDbContext.cs`

**Approach:** Definimos las entidades en C#, EF genera la base de datos.

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<PainPoint>(entity =>
    {
        entity.ToTable("PainPoints");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
        // Configuración fluent
    });
}
```

**Ventaja:** Versionado de base de datos mediante migraciones, portabilidad.

### 6. Blazor WebAssembly

**¿Por qué Blazor WASM?**

- C# en el frontend (sin JavaScript)
- Ejecución en el navegador (SPA)
- Reutilización de código con el backend
- Type-safety end-to-end

**Componentes:**
- `SubmitPain.razor`: Captura de input, validación, HTTP POST
- `PainWall.razor`: SignalR, renderizado dinámico, UI reactiva

---

## 🎓 Temas para Discutir en Clase

### 1. Escalabilidad
- ¿Qué pasa si 1000 usuarios se conectan simultáneamente?
- ¿Cómo escalar SignalR? (Azure SignalR Service, Redis backplane)

### 2. Seguridad
- ¿Deberíamos validar la entrada del usuario?
- ¿Cómo prevenir spam de dolores?
- ¿Autenticación y autorización?

### 3. Rendimiento
- ¿Paginación para 10,000 dolores?
- ¿Caché en el cliente?
- ¿Comprimir datos transmitidos por SignalR?

### 4. Testing
- ¿Cómo hacer unit tests de los endpoints?
- ¿Cómo testear SignalR?
- ¿Integration tests del flujo completo?

### 5. Deployment
- ¿Dónde hospedar el backend? (Azure App Service, AWS, Docker)
- ¿Dónde hospedar el frontend? (Azure Static Web Apps, Netlify)
- ¿CI/CD pipeline?

---

## 📚 Recursos Adicionales

### Documentación Oficial
- [Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/)

### Tailwind CSS
- [Documentación](https://tailwindcss.com/docs)
- [Playground](https://play.tailwindcss.com/)

---

## 🐛 Solución de Problemas

### Error: "No se puede conectar a la base de datos"
- **Causa:** El archivo `painpoints.db` no existe o no tiene permisos.
- **Solución:** Eliminar el archivo si existe y reiniciar el servidor. EF Core lo recreará.

### Error: "SignalR connection failed"
- **Causa:** El cliente intenta conectarse antes de que el servidor esté listo.
- **Solución:** Asegurarse de que el servidor esté ejecutándose en `https://localhost:7001` antes de iniciar el cliente.

### Error de CORS
- **Causa:** El navegador bloquea peticiones cross-origin.
- **Solución:** Verificar que `Program.cs` tenga configurado CORS correctamente (ya está incluido).

### Estilos de Tailwind no se aplican
- **Causa:** Tailwind CDN no cargó o hay problemas de caché.
- **Solución:** Hacer Ctrl+F5 para refrescar sin caché, o verificar la consola del navegador.

---

## 🔮 Mejoras Futuras (Para los Estudiantes)

1. **Autenticación**
   - Añadir Identity para que los usuarios se registren
   - Asociar dolores a usuarios específicos

2. **Categorización**
   - Permitir etiquetar dolores por categoría (Frontend, Backend, DevOps, etc.)
   - Filtrar el muro por categoría

3. **Votación**
   - Upvote/downvote en dolores
   - Ordenar por popularidad

4. **Análisis**
   - Dashboard con estadísticas
   - Gráficos de dolores más comunes
   - Export a Excel/PDF

5. **Notificaciones**
   - Notificaciones push cuando hay un nuevo dolor
   - Email digest diario

6. **Moderación**
   - Panel de admin para aprobar/rechazar dolores
   - Detección de contenido inapropiado

7. **Multiidioma**
   - Soporte para inglés, español, portugués
   - Localización con .resx files

---

## 👨‍💻 Autor

Este proyecto fue creado con fines **educativos** para enseñar arquitectura de software moderna en clases de .NET y Blazor.

---

## 📄 Licencia

Este proyecto es de código abierto y está disponible para uso educativo.

---

## 🙏 Agradecimientos

- Comunidad de .NET
- Equipo de Blazor
- Tailwind CSS

---

**¡Feliz Codificación y Enseñanza! 🚀**
