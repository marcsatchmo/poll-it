# Otros Problemas Potenciales - CORS y Conectividad

Basado en el análisis completo del código, aquí hay otros problemas potenciales que PODRÍAN estar ocurriendo además del CORS:

---

## 1. **Problemas de Origen Exacto (Case-Sensitive)**

### Síntoma:
```
CORS policy: Access-Control-Allow-Origin header mismatch
Expected origin !== actual origin
```

### Causa Potencial:
El origen exacto enviado por el cliente NO coincide exactamente con lo hardcodeado en Program.cs.

### Verificación:
En la consola del navegador (F12), Network tab:
- Mira la columna "Origin" en las peticiones
- Compara con el valor hardcodeado en Program.cs:
  ```csharp
  var productionOrigin = "https://orange-moss-00032b10f.1.azurestaticapps.net";
  ```

### Solución:
Si el origen es diferente (ej: con trailing slash, puerto, protocolo diferente):
```csharp
// ANTES (incorrecto si hay variación)
var hardcodedOrigins = new[]
{
    "https://orange-moss-00032b10f.1.azurestaticapps.net",
    // ...
};

// DESPUÉS (más robusto)
var hardcodedOrigins = new[]
{
    "https://orange-moss-00032b10f.1.azurestaticapps.net",
    "https://orange-moss-00032b10f.1.azurestaticapps.net/", // Con slash
    // ...
};
```

---

## 2. **Credenciales y AllowCredentials**

### Síntoma:
```
Credentials mode is 'include', but 'Access-Control-Allow-Credentials' header is missing
```

### Causa Potencial:
El cliente está enviando credenciales (`credentials: 'include'`) pero el servidor no tiene `AllowCredentials()` habilitado.

### Verificación en el Cliente:
Busca en PainWall.razor o en el HttpClient setup:
```csharp
// ¿Está usando credenciales?
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(apiBaseUrl)
    // ¿Hay algún lugar donde se agrega 'credentials: include'?
});
```

### Estado Actual:
✅ Program.cs tiene `.AllowCredentials()` en la política CORS
✅ El middleware manual agrega `Access-Control-Allow-Credentials: true`

**Conclusión:** Esto está bien configurado.

---

## 3. **Content-Type Header en POST**

### Síntoma:
```
CORS policy: Request header 'content-type' is not allowed by Access-Control-Allow-Headers
```

### Causa Potencial:
El cliente envía Content-Type en POST pero no está permitido en los headers CORS.

### Verificación:
En Network tab (F12), headers de la petición POST:
```
Content-Type: application/json
```

### Estado Actual:
✅ Program.cs permite `Content-Type` en `Access-Control-Allow-Headers`
✅ Middleware manual también lo permite

**Conclusión:** Esto está bien configurado.

---

## 4. **Host Header Mismatch**

### Síntoma:
```
400 Bad Request - Invalid Host header
```

### Causa Potencial:
El client envía un Host header diferente al configurado en Azure App Service.

### Verificación:
En Network tab (F12), Request Headers:
```
Host: poll-it-abe8chfgghe9gyb7.eastus-01.azurewebsites.net
```

¿Coincide con la URL que estás llamando? Si hay un redirect o proxy, podría cambiar.

### Solución:
Si ves un Host diferente:
1. Verifica que ApiBaseUrl en appsettings.json sea exactamente correcto
2. Asegúrate de que no hay proxies intermedios cambiando el Host

---

## 5. **SignalR Negociación (Conexión Persistente)**

### Síntoma:
```
WebSocket connection to 'wss://...' failed
O
SignalR: Connection could not be established
```

### Causa Potencial:
El cliente no puede establecer una conexión persistente con el hub de SignalR.

### Verificación en PainWall.razor:
```csharp
hubConnection = new HubConnectionBuilder()
    .WithUrl(hubUrl)  // ← ¿Apunta a la URL correcta?
    .WithAutomaticReconnect()
    .Build();
```

### Posibles Problemas:
1. **URL del Hub Incorrecta**: Asegúrate de que `hubUrl` sea:
   ```
   https://poll-it-abe8chfgghe9gyb7.eastus-01.azurewebsites.net/painhub
   ```

2. **SignalR Negotiation Endpoint**: SignalR primero intenta `/painhub/negotiate` (CORS-protected)

### Solución:
Si SignalR no conecta:
```csharp
// En PainWall.razor, agrega logging
hubConnection.On("Reconnecting", () => {
    Console.WriteLine("SignalR: Intentando reconectar...");
});

hubConnection.On("Reconnected", (string connectionId) => {
    Console.WriteLine($"SignalR: Conectado - {connectionId}");
});

hubConnection.On("Closed", (Exception exception) => {
    Console.WriteLine($"SignalR: Error - {exception?.Message}");
});
```

---

## 6. **Protocolo HTTP vs HTTPS**

### Síntoma:
```
Mixed Content: The page was loaded over HTTPS, but requested an insecure resource over HTTP
```

### Causa Potencial:
El cliente (Static Web Apps) está en HTTPS pero intenta conectar a HTTP.

### Verificación:
En appsettings.json:
```json
{
  "ApiBaseUrl": "https://poll-it-abe8chfgghe9gyb7.eastus-01.azurewebsites.net/"
}
```

¿Es HTTPS? ✅

**Conclusión:** Esto está bien configurado.

---

## 7. **Azure Static Web Apps Proxy**

### Síntoma:
```
CORS error que no tiene sentido - headers están presentes en Azure App Service
pero el navegador aún rechaza
```

### Causa Potencial:
Azure Static Web Apps actúa como proxy y está reescribiendo o bloqueando headers CORS.

### Verificación:
¿Tienes configurada una ruta proxy en `staticwebapp.config.json`?

```json
{
  "routes": [
    {
      "route": "/api/*",
      "allowedRoles": ["anonymous"],
      "rewrite": "/api/" // ← Esto podría estar causando problemas
    }
  ]
}
```

### Posible Solución:
Si estás usando proxying en Static Web Apps, asegúrate de que:
1. **NO está reescribiendo el Host header**
2. **NO está eliminando headers CORS**
3. **Está permitiendo OPTIONS**

Ejemplo de configuración segura:
```json
{
  "routes": [
    {
      "route": "/api/*",
      "allowedRoles": ["anonymous"],
      "serve": "/index.html",  // Fallback, no rewrite
      "statusCode": 200
    }
  ]
}
```

---

## 8. **Network Latency o Timeout**

### Síntoma:
```
ERR_TIMED_OUT
o
Request timeout
```

### Causa Potencial:
La petición tarda demasiado en responder (Azure App Service está lento o no responde).

### Verificación en F12:
¿Cuánto tiempo tarda la petición?
- 0-200ms: Normal
- 200-1000ms: Aceptable
- >1000ms: Lento

### Solución:
1. Verificar Azure App Service está corriendo
2. Verificar que la base de datos SQLite es accesible
3. Aumentar timeout en el cliente:
   ```csharp
   builder.Services.AddScoped(sp => new HttpClient 
   { 
       BaseAddress = new Uri(apiBaseUrl),
       Timeout = TimeSpan.FromSeconds(30)  // Default es 100 segundos, pero explícito es mejor
   });
   ```

---

## 9. **Firewall o Rate Limiting**

### Síntoma:
```
429 Too Many Requests
o
403 Forbidden
```

### Causa Potencial:
Azure o IIS está limitando las peticiones.

### Verificación en F12:
¿Qué status code devuelve?
- 429: Rate limit
- 403: Forbidden
- 401: Unauthorized

### Solución:
Verificar en Azure App Service > App Service Plan > Properties si hay rate limiting habilitado.

---

## 10. **Database Locking (SQLite)**

### Síntoma:
```
500 Internal Server Error - Database is locked
```

### Causa Potencial:
SQLite puede tener problemas de concurrencia en Azure App Service.

### Verificación en Logs:
Azure App Service > Log Stream > Ver si hay errores de SQLite.

### Solución:
Usar Azure SQL Database en lugar de SQLite para producción:
```csharp
// ACTUALMENTE:
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlite("Data Source=painpoints.db"));

// RECOMENDADO PARA PRODUCCIÓN:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PainPointDbContext>(options =>
    options.UseSqlServer(connectionString));
```

---

## 11. **CORS Preflight Cache (Browser Cache)**

### Síntoma:
```
CORS error, aunque has cambiado la configuración
```

### Causa Potencial:
El navegador cachea la respuesta de preflight OPTIONS.

### Solución:
1. **Hard refresh**: Ctrl+Shift+R (o Cmd+Shift+R en Mac)
2. **Limpiar caché**: F12 > Application > Clear storage > Clear site data
3. **Incognito window**: Abre en modo incógnito

---

## 12. **SignalR Protocol Mismatch**

### Síntoma:
```
SignalR: Cannot negotiate compatible protocol
```

### Causa Potencial:
El cliente y servidor no se ponen de acuerdo en qué protocolo usar (JSON, MessagePack).

### Verificación:
En PainWall.razor:
```csharp
hubConnection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithHubProtocol(new JsonHubProtocol())  // Explícito
    .WithAutomaticReconnect()
    .Build();
```

### Solución:
Asegúrate de que el servidor soporta JSON:
```csharp
builder.Services.AddSignalR()
    .AddJsonProtocol();  // Agregado explícitamente
```

---

## Plan de Diagnóstico Progresivo

Si después del deploy sigues viendo CORS o errores:

1. **Primero**: Abre F12 (Network tab) y verifica:
   - ¿Qué status code devuelve OPTIONS? (debe ser 204)
   - ¿Qué headers está recibiendo el navegador?
   - ¿Cuál es el origen exacto enviado vs esperado?

2. **Segundo**: Revisa Azure App Service logs:
   ```bash
   az webapp log tail --resource-group <group> --name poll-it-abe8chfgghe9gyb7
   ```

3. **Tercero**: Si hay error 500, verifica:
   - Database connectivity
   - Configuration loaded correctly
   - Exceptions en los logs

4. **Cuarto**: Si hay timeout, verifica:
   - App Service está en modo "Always On"
   - Pricing tier no es F1 (muy lento)

---

## Resumen de Estado Actual

| Componente | Status | Riesgo |
|------------|--------|--------|
| CORS Policy | ✅ Fixed | Bajo |
| AllowedHosts | ✅ Fixed | Bajo |
| web.config | ✅ OK | Bajo |
| SignalR Config | ✅ OK | Medio (ver #5) |
| Database (SQLite) | ⚠️ Probablemente OK | Medio (considerar SQL DB) |
| Azure Static Web Apps Config | ❓ Desconocido | Medio |
| Client Config | ✅ OK | Bajo |

---

## Pasos Recomendados

1. **Deploy** los cambios CORS a Azure App Service
2. **Esperar** 5 minutos para que se propaguen
3. **Abrir** https://orange-moss-00032b10f.1.azurestaticapps.net en el navegador
4. **F12 > Network > Reload**
5. **Buscar** la petición a `/api/painpoints`
6. **Verificar** que OPTIONS devuelve 204 con headers CORS
7. **Si OK**: El error está resuelto
8. **Si no OK**: Compartir screenshot de F12 para investigar más
