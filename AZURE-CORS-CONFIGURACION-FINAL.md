# Configuración Final CORS para Azure App Service - CRÍTICO

## Problema Diagnosticado

El error CORS persistente: **"No 'Access-Control-Allow-Origin' header is present"** se debe a que **`ASPNETCORE_ENVIRONMENT` NO está configurado como "Production"** en Azure App Service.

**Consecuencia**: Cuando la aplicación inicia, el archivo `appsettings.Production.json` NO se carga, por lo que `Cors:AllowedOrigins` queda vacío y se usa el fallback de desarrollo (localhost), que rechaza al frontend en Azure Static Web Apps.

---

## Solución: Configurar Variables de Entorno en Azure App Service

### PASO 1: Acceder a Azure App Service

1. Abre [Azure Portal](https://portal.azure.com)
2. Busca tu **App Service** (nombre: `poll-it-abe8chfgghe9gyb7`)
3. En el panel izquierdo, ve a **Settings → Configuration**

### PASO 2: Agregar Variable de Entorno ASPNETCORE_ENVIRONMENT

1. Haz clic en **New application setting**
2. Completa los datos:
   - **Name**: `ASPNETCORE_ENVIRONMENT`
   - **Value**: `Production`
3. Haz clic en **OK**
4. Haz clic en **Save** en la parte superior (aparecerá un cuadro de confirmación)
5. Espera a que se confirme el cambio (toma 10-30 segundos)

**Resultado esperado**: La aplicación se reiniciará automáticamente con esta variable.

### PASO 3: Verificar que appsettings.Production.json sea Accesible

Asegúrate de que:
- ✅ El archivo `Poll-it.Server/appsettings.Production.json` esté en el repositorio
- ✅ El archivo NO esté en `.gitignore` (es seguro publicarlo, solo contiene la URL del frontend que es pública)
- ✅ El archivo contenga `Cors:AllowedOrigins` con la URL correcta del Static Web Apps

Contenido esperado:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://orange-moss-00032b10f.1.azurestaticapps.net"
    ]
  }
}
```

---

## Explicación Técnica del Problema y Solución

### ¿Por qué ocurría el error?

**Configuración en Program.cs**:
```csharp
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] 
    { 
        "https://localhost:7219",  // Fallback para desarrollo
        "http://localhost:5048"
    };
```

- Si `Cors:AllowedOrigins` NO existe en la configuración, se usa el fallback (localhost)
- En **Desarrollo**: Se lee de `appsettings.json` (desarrollo local)
- En **Producción**: Debe leerse de `appsettings.Production.json`

**Problema**: Si `ASPNETCORE_ENVIRONMENT` no es "Production", .NET ignorará `appsettings.Production.json` y solo cargará `appsettings.json` (que NO tiene Cors:AllowedOrigins), causando que se use el fallback localhost.

### ¿Cómo lo corrige la solución?

1. Configurar `ASPNETCORE_ENVIRONMENT=Production` en Azure App Service
2. .NET carga automáticamente `appsettings.Production.json`
3. Se lee `Cors:AllowedOrigins` con la URL real del Static Web Apps
4. `app.UseCors("AllowBlazorClient")` aplica la política correcta
5. Los headers CORS se añaden correctamente a la respuesta
6. El navegador acepta la petición cross-origin ✅

---

## Cambios en el Código (Program.cs)

Se agregaron tres mejoras clave:

### 1. **Logging del Entorno al Iniciar**
```csharp
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"[STARTUP] Entorno: {environment}");
```
→ Ayuda a verificar que ASPNETCORE_ENVIRONMENT está configurado correctamente

### 2. **Logging Detallado de Orígenes Permitidos**
```csharp
Console.WriteLine($"[CORS CONFIG] Orígenes permitidos configurados en entorno '{environment}':");
foreach (var origin in allowedOrigins)
{
    Console.WriteLine($"  - {origin}");
}
```
→ Permite verificar en los logs de Azure que se cargó la configuración correcta

### 3. **Middleware de Debugging CORS**
```csharp
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        Console.WriteLine($"[CORS DEBUG] Petición recibida: {origin}");
    }
    await next();
    if (context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
    {
        Console.WriteLine($"[CORS DEBUG] ✓ ACEPTADO");
    }
    else
    {
        Console.WriteLine($"[CORS DEBUG] ✗ RECHAZADO");
    }
});
```
→ Registra en tiempo real si cada petición fue aceptada o rechazada y por qué

### 4. **Comentario Crítico en app.UseCors()**
```csharp
// CRÍTICO: Especificar exactamente el nombre de la política
// Si no se especifica el nombre, app.UseCors() sin argumentos genera política anónima muy restrictiva
// DEBE coincidir exactamente con builder.Services.AddCors() → options.AddPolicy("AllowBlazorClient", ...)
app.UseCors("AllowBlazorClient");
```
→ Asegura que se use la política correcta (esto YA estaba bien configurado)

---

## Verificación Post-Configuración

### 1. Verificar en Azure Portal

Después de guardar:
1. Ve a **App Service → Log stream** en Azure Portal
2. Verifica que aparezcan los logs:
   - `[STARTUP] Entorno: Production` ✅
   - `[CORS CONFIG] Orígenes permitidos configurados en entorno 'Production':`
   - `[CORS CONFIG]   - https://orange-moss-00032b10f.1.azurestaticapps.net` ✅

### 2. Verificar desde el Navegador

1. Abre el Static Web Apps (frontend)
2. Abre el navegador **DevTools → Network tab**
3. Intenta crear un "pain point"
4. Busca la petición a `https://poll-it-abe8chfgghe9gyb7.eastus-01.azurewebsites.net/api/painpoints`
5. En la pestaña **Response Headers**, verifica que exista:
   ```
   Access-Control-Allow-Origin: https://orange-moss-00032b10f.1.azurestaticapps.net
   ```

Si ves este header → **¡CORS está funcionando! ✅**

---

## Checklist Final

- [ ] Configuré `ASPNETCORE_ENVIRONMENT=Production` en Azure App Service
- [ ] Verifiqué que `appsettings.Production.json` esté en el repositorio
- [ ] Verifiqué que `appsettings.Production.json` contenga la URL correcta del Static Web Apps
- [ ] Ejecuté un nuevo deploy del backend después de configurar la variable
- [ ] Verifiqué en Azure Portal Log Stream que aparezca `[STARTUP] Entorno: Production`
- [ ] Verifiqué en DevTools que el header `Access-Control-Allow-Origin` esté presente

---

## Troubleshooting Adicional

Si aún ves el error CORS después de estos pasos:

1. **Verificar que el deploy incluyó appsettings.Production.json**
   ```bash
   # En Azure Portal → App Service → Development tools → Kudu Console
   # Navega a: D:\home\site\wwwroot
   # Verifica que exista: appsettings.Production.json
   ```

2. **Forzar un restart del App Service**
   - Ve a Azure Portal
   - App Service → Restart (botón en la parte superior)
   - Espera 30 segundos

3. **Revisar los logs en Kudu**
   - Azure Portal → App Service → Advanced tools → Go
   - Ve a **Debug console → LogFiles**
   - Abre el archivo más reciente y busca errores

4. **Verificar que la URL del Static Web Apps sea exacta**
   - En Azure Portal → Static Web Apps → Overview
   - Copia la URL exacta (ej: `https://orange-moss-00032b10f.1.azurestaticapps.net`)
   - Asegúrate que esté en `appsettings.Production.json` sin espacios extras

---

## Referencias

- [ASP.NET Core - Environment-based Appsettings](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#appsettingsjson)
- [Azure App Service - App settings and connection strings](https://learn.microsoft.com/en-us/azure/app-service/configure-common?tabs=portal)
- [ASP.NET Core - CORS in Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-8.0#cors)
