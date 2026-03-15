# 🎨 Poll-it - Diagramas y Visualización

## 📐 Arquitectura General del Sistema

```
┌────────────────────────────────────────────────────────────────────────┐
│                          POLL-IT SYSTEM OVERVIEW                        │
├────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────────────────────────┐                                   │
│  │   NAVEGADOR DEL USUARIO         │                                   │
│  │  ┌──────────────────────────┐   │                                   │
│  │  │  SubmitPain.razor        │   │  ← Captura input                  │
│  │  │  /submit                 │   │                                   │
│  │  └──────────────────────────┘   │                                   │
│  │  ┌──────────────────────────┐   │                                   │
│  │  │  PainWall.razor          │   │  ← Visualiza en vivo              │
│  │  │  /wall                   │   │                                   │
│  │  └──────────────────────────┘   │                                   │
│  └────────┬────────────────────────┘                                   │
│           │ HTTP/REST ↔ WebSocket (SignalR)                           │
│           │                                                            │
│           ▼                                                            │
│  ┌────────────────────────────────────────────┐                       │
│  │      ASP.NET CORE SERVER (port 7001)      │                       │
│  ├────────────────────────────────────────────┤                       │
│  │                                            │                       │
│  │  ┌──────────────────────────────────────┐ │                       │
│  │  │  Program.cs                          │ │                       │
│  │  │  ─────────────────────────────────── │ │                       │
│  │  │  • Middleware pipeline               │ │                       │
│  │  │  • CORS configuration                │ │                       │
│  │  │  • Dependency Injection              │ │                       │
│  │  │  • Minimal API Endpoints             │ │                       │
│  │  └──────────────────────────────────────┘ │                       │
│  │           │                                │                       │
│  │           ├─────────────────┬──────────┬───┤                       │
│  │           │                 │          │   │                       │
│  │           ▼                 ▼          ▼   │                       │
│  │  ┌────────────────┐ ┌────────────┐ ┌────────────┐     │           │
│  │  │ PainHub.cs     │ │ DbContext  │ │ Endpoints  │     │           │
│  │  │ ──────────────│ │ ────────── │ │ ──────────│     │           │
│  │  │ • Connect     │ │ • EF Core  │ │ • POST    │     │           │
│  │  │ • Broadcast   │ │ • SQLite   │ │ • GET     │     │           │
│  │  │ • Disconnect  │ │ • Mapping  │ │ • DELETE  │     │           │
│  │  └────────────────┘ └────────────┘ └────────┬─┘     │           │
│  │                                             │        │           │
│  └─────────────────────────────────────────────┼────────┘           │
│                                               │                      │
│                                               ▼                      │
│                                   ┌──────────────────────┐           │
│                                   │   painpoints.db      │           │
│                                   │   (SQLite Database)  │           │
│                                   │                      │           │
│                                   │  PainPoints Table    │           │
│                                   │  • Id (PK)           │           │
│                                   │  • Text              │           │
│                                   │  • CreatedAt         │           │
│                                   │  • Color             │           │
│                                   └──────────────────────┘           │
│                                                                       │
└───────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Flujo de Comunicación Rio Completo

### 1️⃣ Envío de un Dolor (Submit Flow)

```
Usuario                           Client                          Server                        BD
  │                                 │                              │                            │
  ├─ Abre /submit───────────────────►│                              │                            │
  │                                 │                              │                            │
  ├─ Tipo texto en form ────────────►│                              │                            │
  │                                 │  Storage en component         │                            │
  │                                 │  (painText = "...")           │                            │
  │                                 │                              │                            │
  ├─ Click "Enviar" ────────────────►│                              │                            │
  │                                 │  Validación local             │                            │
  │                                 │  (¿texto no vacío?)           │                            │
  │                                 │  ✓ OK                        │                            │
  │                                 │                              │                            │
  │                                 ├─ POST /api/painpoints ───────►│                            │
  │                                 │  { text: "..." }             │                            │
  │                                 │                              ├─ Crear objeto PainPoint    │
  │                                 │                              ├─ Asignar color aleatorio  │
  │                                 │                              ├─ Timestamp = DateTime.Now │
  │                                 │                              │                            │
  │                                 │                              ├─── INSERT INTO ───────────►│
  │                                 │                              │     PainPoints             │
  │                                 │◄─────────────── 201 Created ──┤     VALUES (...)           │
  │                                 │    { id, text, color, ... }   │                            │
  │                                 │                              │    ◄─── OK ────────────┤
  │                                 │                              │                            │
  │                                 │                              ├─ PainHub.BroadcastNewPain()
  │                                 │                              │                            │
  │◄────────────── "✓ Enviado!" ─────┤                              │                            │
  │  Form limpio                    │ StateHasChanged()            │                            │
  │                                 │                              │                            │
```

### 2️⃣ Visualización en Vivo (Wall Flow)

```
Usuario 1                        Client 1                  SignalR Hub              Client 2                      Usuario 2
  │                               │                            │                        │                            │
  ├─ Abre /wall──────────────────►│                            │                        │                           │
  │                               │ OnInitializedAsync()        │                        │                           │
  │                               │                            │                        │                           │
  │                               ├─ GET /api/painpoints ─────►│                        │                           │
  │                               │◄────────────────────────────┤                        │                           │
  │                               │ [ {pain1}, {pain2} ]        │                        │                           │
  │                               │ pains = [...]               │                        │                           │
  │                               │                            │                        │                           │
  │                               ├─ Connect to Hub────────────►│                        │                           │
  │                               │ wss://localhost/painhub     │                        │                           │
  │                               │◄─────────────────────────────┤                        │                           │
  │                               │ ConnectionId = "ABC123"      │                        │                           │
  │                               │                            │                        │                           │
  │                               ├─ hubConnection.On("Rcv")   │                        │                           │
  │                               │ Registrar listener          │                        │                           │
  │                               │                            │                        ├─ También conectado
  │                               │                            │                        │  ClientId = "XYZ789"
  │                               │                            │                        │
  │ (User 1 enters /submit)       │                            │                        │                            │
  │ ...                           │                            │                        │                            │
  │ (Envía nuevo dolor)           │                            │                        │                            │
  │ ........................       ├─── broadcast newPain ───────►│                        ├─ Enviar a Client 2 ──────►│
  │                               │                            ├─ BroadcastNewPain()    │                            │
  │                               │                            │                        │ "ReceiveNewPain" event     │
  │                               │                            │                        ├─ pains.Add(newPain)       │
  │                               │                            │                        ├─ StateHasChanged()        │
  │                               │                            │                        │ ⚡ Re-render página       │
  │                               │                            │                        │                            │
  │                               │                            │                        │◄────────────────────────────
  │                               │                            │                        │  🎨 Post-it appears!       │
  │◄───────────────────────────────────────────────────────────────────────────────────────────────────────────────│
  │ También lo ve (auto-refresh)   │                            │                        │                            │
  │ (si está en /wall)             │                            │                        │                            │
```

---

## 📊 Diagrama de Componentes

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        COMPONENTES Y DEPENDENCIAS                            │
└─────────────────────────────────────────────────────────────────────────────┘

SHARED LAYER
├─ PainPoint.cs
│  └─ Usado por: Server (DB), Client (UI), Hubs (Broadcast)
│
SERVER LAYER
├─ Program.cs (Configuración)
│  ├─ Configura DbContext
│  ├─ Configura SignalR
│  ├─ Configura CORS
│  └─ Define Endpoints
│
├─ Data/
│  └─ PainPointDbContext.cs
│     ├─ Usa: PainPoint (modelo)
│     └─ Conecta: SQLite BD
│
├─ Hubs/
│  └─ PainHub.cs
│     ├─ Herencia: SignalR Hub
│     ├─ Broadcast: PainPoint
│     └─ Clientes: Todos conectados
│
CLIENT LAYER
├─ Program.cs
│  └─ Configura HttpClient
│
├─ App.razor
│  ├─ RouterComponent
│  ├─ MainLayout
│  └─ NotFound
│
├─ Pages/
│  ├─ Home.razor
│  │  └─ Redirige a /submit
│  │
│  ├─ SubmitPain.razor
│  │  ├─ @inject HttpClient Http
│  │  ├─ Usa: PainPoint (modelo)
│  │  └─ POST /api/painpoints
│  │
│  ├─ PainWall.razor
│  │  ├─ @inject HttpClient Http
│  │  ├─ GET /api/painpoints (carga inicial)
│  │  ├─ SignalR Connection (escuchar updates)
│  │  ├─ On("ReceiveNewPain") callback
│  │  └─ Usa: PainPoint (modelo)
│  │
│  └─ NotFound.razor
│     └─ Página 404
│
└─ Layout/
   ├─ MainLayout.razor
   │  └─ NavMenu.razor
   │     └─ NavMenu.razor.css
   └─ MainLayout.razor.css
```

---

## 🗄️ Diagrama de BD

```
┌──────────────────────────────────────┐
│          PainPoints Table            │
├──────────────────────────────────────┤
│  Id          │ INTEGER (PK, AUTO)    │
│  Text        │ TEXT (NOT NULL, 500)  │
│  CreatedAt   │ DATETIME (NOT NULL)   │
│  Color       │ TEXT (DEFAULT yellow) │
└──────────────────────────────────────┘

Ejemplo de registros:
┌─────┬────────────────────────────┬─────────────────────────┬───────────┐
│ Id  │ Text                       │ CreatedAt              │ Color     │
├─────┼────────────────────────────┼─────────────────────────┼───────────┤
│ 1   │ Bug en login               │ 2025-03-05 14:15:00Z   │ red       │
│ 2   │ Falta documentación        │ 2025-03-05 14:16:30Z   │ blue      │
│ 3   │ Tests toman mucho tiempo   │ 2025-03-05 14:17:15Z   │ pink      │
│ 4   │ API lenta                  │ 2025-03-05 14:18:45Z   │ yellow    │
└─────┴────────────────────────────┴─────────────────────────┴───────────┘
```

---

## 🔀 Ciclo de Vida del Componente PainWall

```
┌─────────────────────────────────────────────────────────────────┐
│            PAINEWALL.RAZOR - LIFECYCLE DIAGRAM                  │
└─────────────────────────────────────────────────────────────────┘

User navigates to /wall
        │
        ▼
OnInitializedAsync()
        │
        ├─ HTTP GET /api/painpoints
        │  └─ pains = [existing pains]
        │
        ├─ new HubConnectionBuilder()
        │  .WithUrl("/painhub")
        │  .Build()
        │
        ├─ hubConnection.On<PainPoint>("ReceiveNewPain", (pain) => {
        │    pains.Add(pain)
        │    StateHasChanged()
        │  })
        │
        └─ await hubConnection.StartAsync()
                        │
                        ▼
                 [Connected to Hub]
                        │
      ┌─────────────────┼─────────────────┐
      │                 │                 │
      │        Waiting for events         │
      │                 │                 │
      │                 │
      │    Other client sends pain
      │                 │
      │                 ▼
      │    🎤 PainHub.BroadcastNewPain()
      │                 │
      │    ┌────────────┴────────────┐
      │    │                         │
      │    ▼ (to all clients)        ▼
      │  Client 1               Client 2
      │    │                     (this one)
      │    ▼                     │
      │  hubConnection.On()      │
      │  ("ReceiveNewPain")      │
      │  │                       │
      │  ▼                       │
      │  pains.Add(newPain)      ▼
      │  StateHasChanged()    StateHasChanged()
      │  │
      │  ▼
      │  🔄 Re-render component
      │  │
      │  ▼
      │  💥 New Post-it appears on screen!
      │
      └─ User closes page or browser closes
             │
             ▼
      DisposeAsync()
             │
             ├─ hubConnection.DisposeAsync()
             └─ [Disconnected from Hub]
```

---

## 🔐 Seguridad: CORS Flow

```
┌──────────────────────────────────────────────────────────────────┐
│              CORS (Cross-Origin Resource Sharing)               │
└──────────────────────────────────────────────────────────────────┘

CLIENT REQUEST
┌─ Origin: http://localhost:3000
├─ Method: POST
├─ Path: /api/painpoints
└─ Headers: Content-Type: application/json

           │
           ▼

SERVER (Program.cs)
┌─ app.UseCors("AllowAll")
│
├─ Check: Does policy allow this origin?
│  └─ ✅ AllowAnyOrigin() → YES
│
└─ Check: Does policy allow this method?
   └─ ✅ AllowAnyMethod() → YES

           │
           ▼

RESPONSE WITH CORS HEADERS
├─ Access-Control-Allow-Origin: *
├─ Access-Control-Allow-Methods: POST, GET, DELETE
├─ Access-Control-Allow-Headers: Content-Type
└─ 200 OK (or 201 Created)

           │
           ▼

BROWSER
└─ ✅ Request allowed (CORS check passed)
   └─ Process response normally
```

---

## 📡 WebSocket Connection (SignalR)

```
┌──────────────────────────────────────────────────────────────────┐
│        SIGNALR WEBSOCKET CONNECTION LIFECYCLE                    │
└──────────────────────────────────────────────────────────────────┘

CLIENT                          SERVER
  │                                │
  ├──── HTTP HANDSHAKE ───────────►│
  │  (Upgrade to WebSocket)        │
  │◄──── 101 Switching Protocols ──┤
  │                                │
  ├───────► WebSocket Open ◄───────┤
  │ ConnectionId = "ABC123"         │
  │                                │
  ├── Send: {"type": 1, ...} ─────►│
  │ (Join group "room1")           │
  │                                │
  │◄── Broadcast: NewPainMessage ──┤
  │    {"type": 3, ...}            │
  │                                │
  │ ...waiting... (connection alive)
  │                                │
  ├── (after 30 sec, heartbeat) ──►│ (keep-alive)
  │◄── Pong response ──────────────┤
  │                                │
  │                                │
  │  (User sends new pain point)   │
  │                                │
  ├─────────────────────────────────► Broadcast to ALL
  │◄─ Event: NewPain (callback) ───┤
  │                                │
  ├─ User closes page              │
  │                                │
  ├──── WebSocket Close ───────────►│
  │◄──── Connection Closed ────────┤
  │                                │

CONNECTION STATES:
┌─────────────────────────────────────┐
│   Disconnected (Initial)            │
│           │                         │
│           ▼                         │
│   Connecting (Handshake)            │
│           │                         │
│           ▼                         │
│   Connected (Ready) ◄────────┐      │
│           │                  │      │
│           ├──► Auto-Reconnect
│           │    (on network loss)
│           │                  │
│           ▼                  │
│   Reconnecting ──────────────┘
│           │
│           ▼
│   Disconnected (Closed)
│
└─────────────────────────────────────┘
```

---

## 📈 Secuencia HTTP Completa

```
REQUEST → SERVER → RESPONSE

1. CLIENT POST /api/painpoints
   POST /api/painpoints HTTP/1.1
   Host: localhost:7001
   Content-Type: application/json
   Content-Length: 52
   
   {"text":"Problema con BD"}    ← Body

                   ↓ (Server recibe)

2. SERVER PROCESSING
   a. Desserializar JSON
   b. Validar texto (no vacío, ≤500 chars)
   c. Crear PainPoint:
      - Id: auto-generated (5)
      - Text: "Problema con BD"
      - CreatedAt: 2025-03-05T14:23:10.123Z
      - Color: "yellow" (random)
   d. Guardar en BD:
      INSERT INTO PainPoints VALUES (5, "Problema con BD", ...)
   e. Ejecutar broadcast:
      PainHub.BroadcastNewPain(newPainPoint)

                   ↓ (Server envía)

3. CLIENT RESPONSE
   HTTP/1.1 201 Created
   Content-Type: application/json
   Content-Length: 122
   
   {
     "id": 5,
     "text": "Problema con BD",
     "createdAt":"2025-03-05T14:23:10.123Z",
     "color": "yellow"
   }

                   ↓ (Client recibe)

4. CLIENT HANDLING
   if (response.IsSuccessStatusCode) {
     ShowMessage("✓ Dolor enviado");
     ClearForm();
   }
```

---

## 🎯 Mapeo de Archivos → Responsabilidades

```
FILES                          RESPONSABILIDAD              CAPAS
═══════════════════════════════════════════════════════════════════════

PainPoint.cs               ← Modelo de dominio           [SHARED]
                              Qué es un dolor?            
                              Propiedades                 
                              ────────────────────────

PainPointDbContext.cs      ← Persistencia               [SERVER]
                              Mapeo O/R                  
                              Validaciones BD            
                              ────────────────────────

PainHub.cs                 ← Comunicación RT            [SERVER]
                              SignalR events             
                              Broadcast                 
                              ────────────────────────

Program.cs (Server)        ← Configuración/API         [SERVER]
                              Middleware                
                              Endpoints                 
                              D.I.                      
                              ────────────────────────

Program.cs (Client)        ← Configuración/Setup       [CLIENT]
                              HttpClient                
                              Rutas                     
                              ────────────────────────

SubmitPain.razor           ← Formulario/UI             [CLIENT]
                              Entrada de datos          
                              Validación local          
                              POST HTTP                 
                              ────────────────────────

PainWall.razor             ← Visualización/UI          [CLIENT]
                              Carga inicial             
                              SignalR listener          
                              Real-time updates         
                              ────────────────────────

App.razor                  ← Router/Layout             [CLIENT]
                              Enrutamiento              
                              Componente raíz           
                              ────────────────────────
```

---

## 🔗 Dependencias entre Capas

```
┌──────────────────────────────────────────┐
│         DEPENDENCY HIERARCHY             │
└──────────────────────────────────────────┘

CLIENT LAYER (Blazor WASM)
    │
    ├─ Depends on: SHARED (PainPoint)
    ├─ Depends on: HTTP/REST (to Server)
    ├─ Depends on: WebSocket/SignalR (to Server)
    │
    └─ Independent of: Database, EF Core
    
         ▲
         │ HTTP Communication
         │
         
SERVER LAYER (ASP.NET Core)
    │
    ├─ Depends on: SHARED (PainPoint)
    ├─ Depends on: EF Core (ORM)
    ├─ Depends on: SQLite (Driver)
    ├─ Depends on: SignalR (Hub)
    │
    └─ Independent of: Client Framework (could be React, etc)
    
         ▲
         │ Database Communication
         │
         
DATA LAYER (Database)
    │
    ├─ Depends on: Nothing (independent)
    │
    └─ Serves: Server queries via EF Core


SHARED LAYER (Models)
    │
    ├─ Depends on: Nothing (pure data)
    │
    └─ Used by: Client & Server
```

---

**Diagrama actualizado**: Marzo 5, 2025
