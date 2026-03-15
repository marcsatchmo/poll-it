# Guía Completa: Despliegue de Poll-it en Azure con GitHub Actions

## Introducción

Esta guía te ayudará a desplegar tu aplicación Poll-it en Azure usando GitHub Actions. Tu proyecto está dividido en:
- **Backend**: Poll-it.Server (API REST + SignalR) → Azure App Service
- **Frontend**: Poll-it.Client (Blazor WebAssembly) → Azure Static Web Apps

Ambos se despliegan desde el mismo repositorio GitHub usando workflows separados.

---

## Parte 1: Crear el Recurso de Azure App Service (Backend)

### Paso 1.1: Acceder a Azure Portal

1. Ve a https://portal.azure.com
2. Inicia sesión con tu cuenta de Azure
3. Si no tienes una suscripción, crea una (incluye créditos de prueba)

### Paso 1.2: Crear un nuevo App Service

1. En el portal, busca **App Services** en la barra de búsqueda
2. Haz clic en **+ Create** (Crear)
3. Completa el formulario de creación:
   - **Nombre**: `poll-it-backend` (o el nombre que prefieras)
   - **Publicar**: Selecciona **Code**
   - **Runtime stack**: Selecciona **.NET 8** (o la versión que uses)
   - **Sistema operativo**: **Linux** (más económico)
   - **Región**: Elige la más cercana a tus usuarios
   - **Plan de App Service**: Crea uno nuevo o usa uno existente (selecciona Free o B1 para pruebas)

4. Haz clic en **Review + Create** y luego **Create**

### Paso 1.3: Descargar el Perfil de Publicación

1. Una vez que se crea el App Service, ve a su página principal
2. En el menú de la izquierda, busca **Deployment center**
3. Selecciona la pestaña **FTPS credentials** o baja hasta encontrar **Download publish profile**
4. Haz clic en **Download publish profile** - se descargará un archivo `.PublishSettings`
5. Guarda este archivo en un lugar seguro (lo necesitarás para crear el secreto)

---

## Parte 2: Crear el Recurso de Azure Static Web Apps (Frontend)

### Paso 2.1: Crear Static Web Apps desde GitHub

Azure puede crear la Static Web App automáticamente y generar el workflow. Te recomendamos este enfoque:

1. En Azure Portal, busca **Static Web Apps**
2. Haz clic en **+ Create** (Crear)
3. Completa el formulario:
   - **Nombre**: `poll-it-frontend` (o el que prefieras)
   - **Plan**: Selecciona **Free** para pruebas
   - **Región**: La misma que tu App Service si es posible

4. En **Deployment details**:
   - **Origen**: **GitHub**
   - Conéctate con tu cuenta de GitHub
   - **Organización**: Selecciona tu cuenta/organización
   - **Repositorio**: Selecciona `poll-it`
   - **Rama**: **main**

5. En **Build Details**:
   - **Build Presets**: **Blazor**
   - **App location**: `Poll-it.Client`
   - **Output location**: `wwwroot`
   - **API location**: Déjalo en blanco (tu API está en un App Service separado)

6. Haz clic en **Review + Create** y luego **Create**

### Paso 2.2: Obtener el Token de Deployment (si es necesario)

Si no usaste GitHub para crear la Static Web App, necesitarás el token:

1. Ve a tu recurso Static Web Apps en Azure Portal
2. En el menú de la izquierda, selecciona **Manage deployment token**
3. Copia el token (será una larga cadena)
4. Guarda este token de forma segura

---

## Parte 3: Configurar Secretos en GitHub

### Paso 3.1: Agregar AZURE_WEBAPP_PUBLISH_PROFILE

1. Ve a tu repositorio en GitHub: https://github.com/marcsatchmo/poll-it
2. Abre **Settings** (Configuración)
3. En el menú de la izquierda, busca **Secrets and variables** → **Actions**
4. Haz clic en **New repository secret**
5. En **Name**, escribe: `AZURE_WEBAPP_PUBLISH_PROFILE`
6. En **Secret**, abre el archivo `.PublishSettings` que descargaste:
   - Abre el archivo con un editor de texto
   - Copia TODO el contenido (incluidas las etiquetas XML)
   - Pégalo en el campo **Secret**
7. Haz clic en **Add secret**

### Paso 3.2: Agregar AZURE_STATIC_WEB_APPS_API_TOKEN

1. De vuelta en **Secrets and variables** → **Actions**
2. Haz clic en **New repository secret**
3. En **Name**, escribe: `AZURE_STATIC_WEB_APPS_API_TOKEN`
4. En **Secret**, pega el token que obtuviste (o Azure ya lo habrá agregado automáticamente si creaste la Static Web App desde GitHub)
5. Haz clic en **Add secret**

---

## Parte 4: Configurar el Backend en Azure Portal

### Paso 4.1: Configurar Variables de Entorno para Producción

1. Ve a tu App Service en Azure Portal (`poll-it-backend`)
2. En el menú de la izquierda, selecciona **Configuration** (Configuración)
3. Haz clic en **+ New application setting**
4. Agrega las siguientes configuraciones:

   | Nombre | Valor | Descripción |
   |--------|-------|-------------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` | Indica que está en producción |
   | `Database` | (dejalo en blanco o configura tu BD) | Cadena de conexión si usas BD externa |

5. Haz clic en **Save**

### Paso 4.2: Habilitar WebSockets para SignalR

1. En el mismo App Service, selecciona **Configuration**
2. Busca **General settings** (Configuración general)
3. Activa **Web sockets**: **On**
4. Haz clic en **Save**

### Paso 4.3: Configurar CORS para aceptar tu Static Web App

1. Ve a tu App Service
2. Abre **CORS** (búscalo en el menú de la izquierda)
3. Agrega la URL de tu Static Web App:
   - La URL será algo como: `https://poll-it-frontend.azurestaticapps.net`
   - Si no sabes exactamente, puedes encontrarla en tu recurso Static Web Apps
4. Haz clic en **Save**

---

## Parte 5: Actualizar la Configuración del Cliente Blazor

### Paso 5.1: Obtener la URL de tu App Service Backend

1. Ve a tu App Service en Azure Portal
2. En la página principal, busca **Default domain**
3. Copia la URL completa (ser�� algo como: `https://poll-it-backend.azurewebsites.net`)

### Paso 5.2: Actualizar Program.cs del Cliente

En `Poll-it.Client/Program.cs`, busca la sección donde se configura el HttpClient y la URL de la API:

```csharp
// Desarrollo
// builder.Services.AddScoped(sp => 
//     new HttpClient { BaseAddress = new Uri("https://localhost:5001") });

// Producción - descomenta y actualiza con tu URL de App Service
builder.Services.AddScoped(sp => 
    new HttpClient { BaseAddress = new Uri("https://poll-it-backend.azurewebsites.net") });
```

Reemplaza `poll-it-backend.azurewebsites.net` con el dominio real de tu App Service.

### Paso 5.3: Actualizar la Conexión SignalR

Si tu código configura la conexión SignalR, asegúrate de que use la misma URL base:

```csharp
var hubConnection = new HubConnectionBuilder()
    .WithUrl("https://poll-it-backend.azurewebsites.net/painHub")
    .WithAutomaticReconnect()
    .Build();
```

---

## Parte 6: Primer Despliegue Manual (Opcional pero Recomendado)

### Paso 6.1: Hacer un Push a Main para Activar los Workflows

1. Asegúrate de que los archivos de workflow estén en `.github/workflows/`:
   - `deploy-backend.yml`
   - `deploy-frontend.yml`

2. Haz un commit y push a la rama `main`:

```bash
git add .github/workflows/deploy-backend.yml
git add .github/workflows/deploy-frontend.yml
git add Poll-it.Client/Program.cs  # (si actualizaste la URL)
git commit -m "feat: add Azure deployment workflows"
git push origin main
```

3. Ve a tu repositorio en GitHub
4. Abre la pestaña **Actions**
5. Deberías ver dos workflows ejecutándose:
   - "Deploy Backend a Azure App Service"
   - "Deploy Frontend a Azure Static Web Apps"

### Paso 6.2: Verificar el Estado de los Despliegues

1. Haz clic en cada workflow para ver los detalles
2. Si hay errores, revisa los logs (mostrarán exactamente qué falló)
3. Los errores más comunes:
   - **Secretos no configurados**: Asegúrate de haberlos agregado correctamente en GitHub
   - **Versión de .NET incorrecta**: Verifica que coincida con tu proyecto
   - **Rutas incorrectas**: Confirma que los paths en el workflow existen

---

## Parte 7: Pruebas Post-Despliegue

### Paso 7.1: Probar el Backend

1. Abre tu navegador y ve a: `https://poll-it-backend.azurewebsites.net/swagger`
2. Deberías ver la documentación de Swagger de tu API
3. Prueba uno de los endpoints para confirmar que funciona

### Paso 7.2: Probar el Frontend

1. Ve a: `https://poll-it-frontend.azurestaticapps.net`
2. Deberías ver tu aplicación Blazor WebAssembly cargada
3. Intenta crear un "pain point" y verifica que se comunique correctamente con el backend

### Paso 7.3: Probar SignalR (si aplica)

1. Abre dos navegadores con la aplicación
2. En uno, crea un nuevo pain point
3. En el otro, deberías ver la actualización en tiempo real
4. Si no funciona, revisa:
   - Que WebSockets esté habilitado en App Service
   - Que CORS esté correctamente configurado
   - Los logs en Azure Application Insights

---

## Solución de Problemas Comunes

### El workflow falla con "Secreto no encontrado"

**Problema**: El workflow intenta acceder a un secreto que no existe.

**Solución**:
1. Ve a Settings → Secrets and variables → Actions en GitHub
2. Confirma que ambos secretos existen:
   - `AZURE_WEBAPP_PUBLISH_PROFILE`
   - `AZURE_STATIC_WEB_APPS_API_TOKEN`
3. Si no existen, créalos siguiendo la Parte 3

### El frontend no se conecta al backend

**Problema**: Ves errores de CORS en la consola del navegador.

**Soluciones**:
1. Verifica que la URL en `Program.cs` sea correcta
2. Confirma que CORS esté habilitado en el App Service con la URL de Static Web Apps
3. Comprueba que el backend está corriendo (ve a su URL + `/swagger`)

### El backend no inicia en Azure

**Problema**: Ves errores en los logs del App Service.

**Soluciones**:
1. Ve a tu App Service → **Log stream** para ver los logs en tiempo real
2. Revisa que `ASPNETCORE_ENVIRONMENT` esté en "Production"
3. Si usas base de datos, verifica la cadena de conexión en Configuration
4. Comprueba la versión de .NET: asegúrate de que el Stack sea correcto

### SignalR no funciona en tiempo real

**Problema**: Las actualizaciones no llegan en tiempo real.

**Soluciones**:
1. Habilita WebSockets en App Service:
   - Ve a Configuration → General settings
   - Busca "Web sockets" y actívalo
2. Verifica que la URL de SignalR sea correcta en el cliente
3. Revisa los logs en Application Insights para más detalles

---

## Monitoreo y Mantenimiento

### Acceder a Application Insights

1. Ve a tu App Service
2. En el menú, busca **Application Insights**
3. Aquí puedes ver:
   - Errores y excepciones
   - Rendimiento y tiempos de respuesta
   - Logs en tiempo real
   - Disponibilidad

### Revisar Logs en Tiempo Real

1. Ve a tu App Service
2. Busca **Log stream** en el menú
3. Aquí verás los logs en vivo mientras la aplicación se ejecuta

### Escalar tu Aplicación

Si necesitas más recursos:

1. Ve a tu App Service
2. En el menú, selecciona **Scale up**
3. Elige un plan más potente (B1, B2, etc.)
4. Los costos aumentarán, pero la aplicación será más rápida

---

## Resumen del Flujo de Despliegue

```
┌─────────────────────────────────────────────────┐
│  Haces Push a la rama Main en GitHub            │
└──────────────────┬──────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
┌──────────────────┐  ┌────────────────────┐
│ Workflow Backend │  │ Workflow Frontend  │
│ (.NET 8 build)   │  │ (Blazor WASM)      │
└────────┬─────────┘  └────────┬───────────┘
         │                     │
         ▼                     ▼
┌──────────────────┐  ┌────────────────────┐
│  Azure App       │  │  Azure Static      │
│  Service         │  │  Web Apps          │
│  (Backend API)   │  │  (Frontend SPA)    │
└──────────────────┘  └────────────────────┘
```

---

## Próximos Pasos

1. ✅ Completa todos los pasos de esta guía
2. ✅ Haz un push a main para activar los workflows
3. ✅ Revisa que ambos despliegues sean exitosos
4. ✅ Prueba tu aplicación en producción
5. ✅ Configura alertas en Application Insights
6. ✅ Implementa un plan de backup para tu base de datos (si aplica)

---

## Notas Importantes

- **Versión de .NET**: Los workflows usan .NET 8.0 por defecto. Si tu proyecto usa otra versión, actualiza `dotnet-version` en ambos archivos workflow.
- **Costos**: Azure ofrece algunos servicios gratuitos, pero verifica tu cuota para evitar cargos inesperados.
- **CORS**: Asegúrate de que CORS esté bien configurado, es una causa común de problemas en producción.
- **Secretos**: Nunca compartas tus secretos de Azure públicamente. GitHub los mantiene seguros.

---

## Soporte

Si encuentras problemas:

1. Revisa los logs en el workflow de GitHub (Actions tab)
2. Revisa los logs en Application Insights (Azure Portal)
3. Verifica que todos los secretos estén correctamente configurados
4. Confirma que las URLs sean correctas en tu código

¡Éxito con tu despliegue! 🚀
