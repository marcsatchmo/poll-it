# ⚡ Poll-it - Guía Rápida

## 🚀 Iniciar la App (30 segundos)

```bash
# Terminal 1 - Backend
cd Poll-it.Server
dotnet run

# Terminal 2 - Frontend
cd Poll-it.Client
dotnet run
```

**Acceso:**
- 🌐 Cliente: `https://localhost:3000` (o puerto asignado)
- 🔗 API: `https://localhost:7001`
- 📊 DB: `painpoints.db` (en la carpeta del servidor)

---

## 🗺️ Mapa Rápido del Proyecto

```
Poll-it.Shared          ← Modelos compartidos (PainPoint.cs)
    ↓
Poll-it.Server          ← API REST + SignalR Hub
    ├─ Data/   → BD (SQLite)
    ├─ Hubs/   → Tiempo real
    └─ Program.cs → Endpoints

Poll-it.Client          ← Blazor WebAssembly
    ├─ Pages/
    │  ├─ SubmitPain.razor  → Formulario (enviar)
    │  ├─ PainWall.razor    → Visualización (ver en vivo)
    │  └─ Home.razor        → Inicio
    └─ Program.cs → Configuración cliente
```

---

## 📤 Flujo de Datos

```
Usuario escribe → SubmitPain.razor 
              → POST /api/painpoints 
              → Server guarda en BD 
              → Broadcast via SignalR 
              → PainWall.razor actualiza 
              → ✨ Aparece en vivo
```

---

## 🔌 API Endpoints

| Método | Ruta | Propósito |
|--------|------|----------|
| `POST` | `/api/painpoints` | Crear nuevo dolor |
| `GET` | `/api/painpoints` | Obtener todos |
| `DELETE` | `/api/painpoints/{id}` | Eliminar |

---

## 📡 SignalR

**URL del Hub:** `wss://localhost:7001/painhub`

**Eventos:**
- `ReceiveNewPain` - Nuevo dolor recibido (server → client)

---

## 🐛 Problemas Comunes

| Problema | Solución |
|----------|----------|
| ❌ Connection refused | ¿Server corriendo? Usa `dotnet run` |
| ❌ CORS blocked | Revisar `UseCors()` en Program.cs |
| ❌ SignalR failed | Verificar URL del hub en cliente |
| ❌ Puerto ocupado | Cambiar en `launchSettings.json` |

---

## 📝 Archivos Importantes

```
Configuración Server:
  → Poll-it.Server/Program.cs
  → Poll-it.Server/Properties/launchSettings.json

Configuración Cliente:
  → Poll-it.Client/Program.cs

Modelo de Datos:
  → Poll-it.Shared/PainPoint.cs
```

---

## 💾 Base de Datos

**Ubicación:** `Poll-it.Server/painpoints.db` (SQLite)

**Tabla:**
```sql
PainPoints (Id, Text, CreatedAt, Color)
```

**Limpiar todo:**
```bash
rm painpoints.db
dotnet ef database update
```

---

## 🛠️ Desarrollo Rápido

```bash
# Compilar
dotnet build

# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore

# Ver versión .NET
dotnet --version
```

---

## 📚 Documentación Completa

Para más detalles, ver: **[DOCUMENTACION-TECNICA.md](DOCUMENTACION-TECNICA.md)**

Para arquitectura: **[ARCHITECTURE.md](ARCHITECTURE.md)**

---

## 🎯 Casos de Uso principales

### Caso 1: Un analista reporta un problema
- Usuario → Página `/submit` → Escribe dolor → Click "Enviar"
- Rest de usuarios ven aparecer en `/wall`

### Caso 2: Visualizar en vivo
- Usuario → Página `/wall` → Ve dolores actuales y nuevos en tiempo real
- SignalR actualiza la pantalla automáticamente

### Caso 3: Limpiar dolores
- Admin → DELETE `/api/painpoints/5` → Dolor eliminado

---

## 🔗 URLs de la App

- **Inicio:** `/` (redirige a `/submit`)
- **Enviar:** `/submit` - Formulario para agregar dolor
- **Muro:** `/wall` - Panel de visualización en vivo
- **404:** `/notfound` - Página no encontrada (automática)

---

## 🎓 Aprendizaje

**Conceptos demostrados:**
- ✅ Architectura 3 capas (Client, Server, Data)
- ✅ Separation of Concerns
- ✅ Inyección de Dependencias
- ✅ Entity Framework Core + DBContext
- ✅ Minimal APIs (.NET)
- ✅ SignalR (comunicación RT)
- ✅ Blazor WebAssembly
- ✅ CORS y seguridad

---

**Creado:** Marzo 5, 2025
