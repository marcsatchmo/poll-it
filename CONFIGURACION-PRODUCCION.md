# Configuración de Producción - Poll-it

## 📋 Resumen

Este documento explica la estrategia de configuración para desplegar **Poll-it Backend** en **Azure App Service** y **Poll-it Frontend** en **Azure Static Web Apps**, manteniendo el repositorio público sin exponer secretos.

---

## 🔍 Diagnóstico: Configuraciones Faltantes

Al analizar el proyecto se identificaron las siguientes configuraciones que necesitan valores en producción:

| Aspecto | Desarrollo Actual | Necesario en Producción |
|--------|------------------|----------------------|
| **Logging** | `Information` (verbose) | `Warning` (optimizado) |
| **Base de Datos** | SQLite local `painpoints.db` | Azure SQL Database |
| **CORS** | `localhost:7219, localhost:5048` | Dominio real Static Web Apps |
| **AllowedHosts** | `*` (sin restricción) | `*.azureappservices.net` |
| **appsettings** | Solo Development | Production + Environment vars |

---

## 📁 Archivos de Configuración

### 1. **appsettings.Development.json** (Desarrollo Local)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```
✅ Se ignorea en .gitignore (contiene posibles secretos locales)

### 2. **appsettings.json** (Valores por defecto)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```
✅ Se commitea (sin datos sensibles)

### 3. **appsettings.Production.json** ⭐ (Nuevo)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*.azureappservices.net",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=painpoints.db"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://tu-app-nombre.azurestaticapps.net"
    ]
  }
}
```
✅ Se commitea **CON PLACEHOLDERS** (sin valores reales)

---

## 🔐 Estrategia de Secretos

### ✅ Lo que SÍ se commitea en Git:
- `appsettings.Production.json` con **placeholders**
- Estructura de configuración
- Comentarios documentando qué reemplazar

### ❌ Lo que NO se commitea:
- `appsettings.Development.json` (en .gitignore)
- Valores reales de secretos (connection strings, API keys)

### 🔄 Cómo se inyectan los secretos:

Los valores reales se configuran como **Environment Variables** directamente en Azure App Service:

```
ConnectionStrings__DefaultConnection = "Server=tcp:mi-servidor.database.windows.net,1433;Initial Catalog=poll-it-db;User Id=admin;Password=XXXXX;"

Cors__AllowedOrigins__0 = "https://mi-app.azurestaticapps.net"
```

**ASP.NET Core automáticamente reemplaza los valores** de `appsettings.Production.json` con las variables de entorno usando la notación `__` (doble guión bajo).

---

## 🚀 Pasos para Configurar en Producción

### Paso 1: En Azure Portal - App Service

1. **Ir a**: App Service > Settings > Environment variables
2. **Agregar las siguientes variables**:

| Variable | Valor |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Tu connection string de Azure SQL |
| `Cors__AllowedOrigins__0` | URL de tu Static Web Apps |

### Paso 2: En Azure Portal - Static Web Apps

1. **Ir a**: Static Web App > Configuration
2. **Agregar variable de entorno**:
   - `REACT_APP_API_URL` = `https://tu-appservice.azurewebsites.net`

### Paso 3: Verificación

Accede a tu App Service y verifica en **Logs** que:
- Dice `Environment: Production`
- Los niveles de logging son `Warning` o superior
- No hay errores de conexión a base de datos

---

## 📝 Modificaciones en Program.cs (Recomendadas)

Para usar dinámicamente la configuración de CORS según el ambiente:

```csharp
// En Program.cs, modificar la sección de CORS:
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "https://localhost:7219", "http://localhost:5048" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## 🗄️ Base de Datos

### Desarrollo:
- SQLite local: `painpoints.db`
- Se genera automáticamente

### Producción:
- Azure SQL Database
- Connection string en variable de entorno

Para migrar:
1. Crear Azure SQL Database
2. Ejecutar migraciones (si usas EF Core Migrations)
3. O dejar que `EnsureCreated()` cree las tablas

---

## ✅ Checklist de Producción

- [ ] Azure App Service creado
- [ ] Azure SQL Database creado
- [ ] Static Web Apps creado
- [ ] Environment variables configuradas en App Service
- [ ] `appsettings.Production.json` con placeholders commiteado
- [ ] Workflows de GitHub Actions configurados
- [ ] Primer deploy realizado
- [ ] Verificar logs en Azure Portal
- [ ] Probar endpoints de la API
- [ ] Probar comunicación en tiempo real (SignalR)

---

## 🔗 Referencias

- [Azure App Service - Application Settings](https://learn.microsoft.com/en-us/azure/app-service/configure-common?tabs=portal)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
- [Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/)
