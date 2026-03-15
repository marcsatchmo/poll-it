using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Poll_it.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient leyendo la URL base desde la configuración por entorno.
//
// CÓMO FUNCIONA LA CONFIGURACIÓN POR ENTORNO EN BLAZOR WASM:
// =============================================================
// Blazor WebAssembly carga automáticamente los archivos de configuración
// desde la carpeta wwwroot en el siguiente orden de prioridad:
//
//   1. wwwroot/appsettings.json          → valores base (producción)
//   2. wwwroot/appsettings.{Environment}.json → sobreescribe según el entorno
//
// El entorno se determina por la variable de build ASPNETCORE_ENVIRONMENT:
//   - En desarrollo local:  carga appsettings.Development.json  → https://localhost:7001/
//   - En producción (Azure): usa appsettings.json como base      → https://TU-APP.azurewebsites.net/
//
// IMPORTANTE: A diferencia de ASP.NET Core del servidor, en Blazor WASM
// estos archivos son archivos estáticos descargados por el browser.
// NO deben contener secretos sensibles porque el usuario puede verlos.
//
// NOTA: SignalR en PainWall.razor usa Navigation.ToAbsoluteUri("/painhub")
// que resuelve relativo al origen del servidor de la SPA, NO a la API.
// Por eso SignalR NO necesita configuración adicional aquí: en producción,
// el cliente Blazor se servirá desde Static Web Apps y conectará al hub
// mediante la URL configurada en ApiBaseUrl.

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException(
        "No se encontró 'ApiBaseUrl' en la configuración. " +
        "Verificá que exista wwwroot/appsettings.json con la clave 'ApiBaseUrl'.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

await builder.Build().RunAsync();
