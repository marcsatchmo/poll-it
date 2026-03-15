# ✅ Poll-it - Verificación y Best Practices

## 🔍 Checklist de Instalación

### Pre-requisitos
- [ ] Windows 10+ / macOS 11+ / Linux Ubuntu 20.04+
- [ ] .NET 10.0 SDK instalado
- [ ] Git instalado
- [ ] 500 MB espacio libre en disco
- [ ] Navegador moderno (Chrome, Firefox, Edge, Safari)

### Verificación de .NET

```bash
# Ejecutar en terminal
dotnet --version

# Debe mostrar algo como: .NET 10.0.x
```

**Si no ves .NET 10:**
1. Descargar desde https://dotnet.microsoft.com/download/dotnet/10.0
2. Instalar
3. Reiniciar terminal

---

## 🚀 Checklist de Ejecución

### Paso 1: Clonar Repo
```bash
git clone <URL-del-repositorio>
cd Poll_it
```
- [ ] Carpeta descargada correctamente
- [ ] Archivo `Poll-it.slnx` existe

### Paso 2: Restaurar Dependencias
```bash
dotnet restore
```
- [ ] Sin errores (puede tomar 1-2 min)
- [ ] Carpeta `packages` creada

### Paso 3: Compilar
```bash
dotnet build
```
- [ ] Compilación exitosa
- [ ] Sin warnings críticos
- [ ] Carpetas `bin/Debug` creadas en cada proyecto

### Paso 4: Ejecutar Server

**Terminal 1:**
```bash
cd Poll-it.Server
dotnet run
```

**Verificaciones:**
- [ ] Mensaje: "Application started"
- [ ] URL: "Now listening on: https://localhost:7001"
- [ ] BD creada: `painpoints.db` existe en carpeta Server
- [ ] Sin errores en la consola

### Paso 5: Ejecutar Cliente

**Terminal 2 (NUEVA terminal):**
```bash
cd Poll-it.Client
dotnet run
```

**Verificaciones:**
- [ ] Mensaje: "Application started"
- [ ] URL: "Now listening on: https://localhost:3000" (o similar)
- [ ] Sin errores en la consola

### Paso 6: Acceder Navegador

**Abrir navegador:**
```
https://localhost:3000
```

**Verificaciones:**
- [ ] Página carga (sin errores 404 o CORS)
- [ ] Ves title "Poll-it"
- [ ] Se redirige a `/submit`

---

## 🧪 Checklist de Funcionalidad

### Feature 1: Enviar un Dolor

**En la página `/submit`:**

```bash
# Pasos
1. Escribe: "Problema test"
2. Click "Enviar"
```

**Verificaciones:**
- [ ] Textarea acepta tu texto
- [ ] Botón "Enviar" está habilitado (no grisado)
- [ ] Mensaje: "✓ Dolor enviado" aparece
- [ ] Textarea se limpia
- [ ] En servidor log aparece: "Dolor guardado"

**Resultado esperado:**
```
[SubmitPain] Dolor enviado exitosamente
  - Id: 1
  - Text: "Problema test"
  - Color: (algún color aleatorio)
```

---

### Feature 2: Visualizar en Tiempo Real

**En otra pestaña o ventana:**

```bash
# URL
https://localhost:3000/wall
```

**Verificaciones:**
- [ ] Página carga
- [ ] Ves el dolor que acabas de enviar como un "Post-it"
- [ ] El Post-it tiene el color asignado

**Prueba en Vivo:**
1. Abre `/wall` en ventana A (navegador)
2. Abre `/submit` en ventana B (navegador)
3. En B, envía nuevo dolor: "Test en Vivo"
4. Verifica que aparece **INSTANTÁNEAMENTE** en A
5. Sin refrescar página (✓ WebSocket funcionando)

**Verificaciones:**
- [ ] Nuevo Post-it aparece sin refrescar
- [ ] Texto: "Test en Vivo"
- [ ] Color: (aleatorio)

---

### Feature 3: Base de Datos

**Verificar archivo BD:**

```bash
# En Terminal del Server
ls -la painpoints.db
# o en Windows:
dir painpoints.db
```

**Verificaciones:**
- [ ] Archivo existe
- [ ] Tamaño: > 0 bytes
- [ ] Timestamp: actualizado recientemente

**Con SQLite CLI (si tienes instalado):**
```bash
sqlite3 painpoints.db
SELECT COUNT(*) FROM PainPoints;
SELECT * FROM PainPoints;
.exit
```

**Verificaciones:**
- [ ] Puedes leer la tabla
- [ ] Registros coinciden con lo que enviaste
- [ ] Timestamps correctos

---

## 🔧 Checklist de Configuración

### CORS

**En `Poll-it.Server/Program.cs`:**

```csharp
builder.Services.AddCors(options => 
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));

app.UseCors("AllowAll");
```

**Verificaciones:**
- [ ] `AddCors` registrado
- [ ] Política "AllowAll" existe
- [ ] `app.UseCors()` llamado ANTES de endpoints

---

### SignalR

**En `Poll-it.Server/Program.cs`:**

```csharp
builder.Services.AddSignalR();
...
app.MapHub<PainHub>("/painhub");
```

**Verificaciones:**
- [ ] `AddSignalR()` llamado
- [ ] Hub mapeado a `/painhub`

**En `Poll-it.Client/Pages/PainWall.razor`:**

```csharp
hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7001/painhub")
    .WithAutomaticReconnect()
    .Build();
```

**Verificaciones:**
- [ ] URL correcta (puerto 7001)
- [ ] AutomaticReconnect habilitado

---

### Base de Datos

**En `Poll-it.Server/Program.cs`:**

```csharp
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));
```

**Verificaciones:**
- [ ] SQLite configurado
- [ ] Connection string correcta
- [ ] Archivo `painpoints.db` se crea en carpeta Server

---

## 🔒 Checklist de Seguridad

### HTTPS

**Verificaciones:**
- [ ] URLs usan `https://` (no `http://`)
- [ ] Certificados autofirmados sin warnings (en desarrollo OK)

### CORS

**Verificaciones:**
- [ ] Solo orígenes permitidos pueden acceder
- [ ] Methods restringidos si es necesario
- [ ] Headers validados

### Validación

**En servidor:**
```csharp
if (string.IsNullOrWhiteSpace(newPain.Text))
    return Results.BadRequest("Text es requerido");
```

- [ ] Validación en servidor (cliente puede bypassearse)
- [ ] Mensajes de error no revelan detalles internos

---

## 📊 Checklist de Rendimiento

### Tiempos Esperados

| Operación | Tiempo Esperado | Tu Tiempo |
|-----------|-----------------|----------|
| POST /api/painpoints | < 100 ms | _____ ms |
| GET /api/painpoints | < 50 ms | _____ ms |
| WebSocket broadcast | < 50 ms | _____ ms |

### Prueba de Carga

**Envía 10 dolores rápidamente:**

```bash
for i in {1..10}; do
  curl -X POST https://localhost:7001/api/painpoints \
    -H "Content-Type: application/json" \
    -d "{\"text\":\"Dolor $i\"}"
done
```

**Verificaciones:**
- [ ] Todos se crean
- [ ] Todos aparecen en `/wall`
- [ ] Sin delays notables

---

## 🐛 Checklist de Debugging

### Logs del Servidor

**En terminal Server, deberías ver:**

```
[SubmitPain] Cliente conectado
[SubmitPain] POST /api/painpoints
[SubmitPain] Dolor guardado: Id=5, Text="...", Color="yellow"
[SubmitPain] Broadcasting nuevoPain
[SubmitPain] Cliente desconectado
```

**Verificaciones:**
- [ ] Logs coinciden con acciones
- [ ] No hay errores 500
- [ ] Timestamps corretos

### Logs del Cliente

**En browser DevTools (F12 → Console):**

**Verificaciones:**
- [ ] Sin errores 404
- [ ] Sin CORS errors
- [ ] Sin errors de parsing JSON

---

## 🔄 Checklist de Integración

### Cliente ↔ Servidor

**Flujo Completo:**

1. [ ] Cliente conecta a API
2. [ ] Cliente conecta a Hub SignalR
3. [ ] Envía dolor → Servidor recibe
4. [ ] Servidor guarda en BD
5. [ ] Servidor notifica a clientes
6. [ ] Cliente actualiza UI

### Base de Datos ↔ Servidor

**Flujo Completo:**

1. [ ] Server crea conexión a SQLite
2. [ ] DbContext mapea tablas
3. [ ] Puedes hacer CRUD operaciones
4. [ ] Datos persisten entre reinicios

---

## 📋 Checklist Pre-Deploy

### Antes de Llevar a Producción

```bash
# Compilar en Release
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish

# Verificar tamaño
du -sh ./publish

# Verificar estructuras
ls -la ./publish/
```

**Verificaciones:**
- [ ] Compilación Release sin warnings
- [ ] Tamaño razonable (< 200 MB)
- [ ] Archivos ejecutables presentes

---

### Cambios Necesarios para Producción

```csharp
// ❌ ANTES (Desarrollo)
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

// ✅ DESPUÉS (Producción)
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**Verificaciones:**
- [ ] Connection strings actualizadas
- [ ] Secrets configurados (no hardcoded)
- [ ] CORS restrictivo (no AllowAnyOrigin)
- [ ] Logging en nivel correcto

---

## 🎯 Best Practices Implementadas

### ✅ Código Limpio

- [ ] Nombres significativos (`PainPoint` vs `p`)
- [ ] Funciones pequeñas (una responsabilidad)
- [ ] Comments explícitos en secciones complejas

### ✅ Seguridad

- [ ] Input validation en servidor
- [ ] HTTPS en todas comunicaciones
- [ ] CORS configurado
- [ ] No expone errores internos

### ✅ Mantenibilidad

- [ ] Separación de Concerns (3 proyectos)
- [ ] Dependency Injection
- [ ] Entity Framework (ORM, no raw SQL)
- [ ] Código documentado

### ✅ Escalabilidad

- [ ] Minimal API (fácil agregar endpoints)
- [ ] SignalR (escalable a muchos clientes)
- [ ] DbContext (fácil cambiar a SQL Server)

### ✅ Testing

- [ ] Tests unitarios (considerar agregar)
- [ ] Manual testing posible
- [ ] Error handling robusto

---

## 📝 Procedimiento de Validación Completa

### Día 1: Setup

```bash
# 1. Verificar .NET
dotnet --version

# 2. Clonar repo
git clone <url>

# 3. Instalar
cd Poll-it
dotnet restore
dotnet build

# ✅ Checkpoint 1: Compilar sin errores
```

### Día 2: Ejecutar

```bash
# Terminal 1
cd Poll-it.Server
dotnet run

# Terminal 2
cd Poll-it.Client
dotnet run

# ✅ Checkpoint 2: Ambos iniciados sin errores
```

### Día 3: Funcional

```bash
# En navegador
https://localhost:3000/submit
# Enviar un dolor
# ✅ Checkpoint 3: /submit funciona

https://localhost:3000/wall
# Ver dolor en vivo
# ✅ Checkpoint 4: /wall funciona
```

### Día 4: Verificaciones Profundas

```bash
# Base de datos
sqlite3 Poll-it.Server/painpoints.db
SELECT COUNT(*) FROM PainPoints;

# ✅ Checkpoint 5: BD tiene datos
```

### Día 5: Integración

- Multi-tab test (dos ventanas)
- Cross-client updates
- Performance bajo carga

---

## 🆘 Si Algo Falla

### Error 1: "Connection Refused"
```
[❌] (Address already in use)
[✅] Cambiar puerto en launchSettings.json
```

### Error 2: "CORS Blocked"
```
[❌] No CORS headers
[✅] Verificar UseCors en Program.cs
```

### Error 3: "SignalR Connection Failed"
```
[❌] Cannot connect to wss://...
[✅] Verificar URL hub y puerto
```

→ Ver más soluciones en [DOCUMENTACION-TECNICA.md - Troubleshooting](DOCUMENTACION-TECNICA.md#troubleshooting)

---

## 📞 Resumen de Checkpoints

| Checkpoint | Qué Verificar | Estado |
|-----------|---------------|--------|
| 1 | Compilación | `[ ]` |
| 2 | Server ejecuta | `[ ]` |
| 3 | Client ejecuta | `[ ]` |
| 4 | /submit funciona | `[ ]` |
| 5 | /wall funciona | `[ ]` |
| 6 | En vivo (WebSocket) | `[ ]` |
| 7 | Base de datos | `[ ]` |
| 8 | Multi-cliente | `[ ]` |

---

**Actualizado**: Marzo 5, 2025
