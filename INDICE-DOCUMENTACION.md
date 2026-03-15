# 📚 Poll-it - Índice de Documentación

> **¿Primer día?** Comienza con [GUIA-RAPIDA.md](#-guía-rápida)  
> **¿Necesitas ejecutar?** Ve a [Cómo Iniciar](#-cómo-iniciar-la-aplicación)  
> **¿Desarrollando?** Lee [DOCUMENTACION-TECNICA.md](#-documentación-técnica-completa)

---

## 📍 Mapa de Documentación

### 🚀 Para Iniciar Rápido
```
┌─ GUIA-RAPIDA.md
│  ├─ Cómo ejecutar (30 seg)
│  ├─ Mapa del proyecto
│  └─ Troubleshooting rápido
│
└─ README.md (original)
   ├─ Descripción general
   └─ Propósito educativo
```

### 📖 Para Entender la Arquitectura
```
┌─ ARCHITECTURE.md (original)
│  ├─ Principios arquitectónicos
│  ├─ Separation of Concerns
│  ├─ SOLID principles
│  └─ Decisiones de diseño
│
└─ DIAGRAMAS-VISUALES.md
   ├─ Flujos de datos
   ├─ Ciclo de vida de componentes
   ├─ WebSocket connections
   └─ Dependencias entre capas
```

### 🛠️ Para Desarrollar
```
┌─ DOCUMENTACION-TECNICA.md (ESTE ES EL MÁS COMPLETO)
├─ Requisitos del sistema
├─ Instalación paso a paso
├─ Estructura completa del proyecto
├─ API REST endpoints
├─ SignalR communication
├─ Base de datos schema
├─ Guía de extensión
├─ Debugging y logging
│ └─ Incluso cómo agregar nueva funcionalidad
└─ Troubleshooting detallado
```

### 🗂️ Estructura de Carpetas
```
Poll_it/
├── 📄 README.md                    ← Descripción general
├── 📄 ARCHITECTURE.md              ← Principios arquitectónicos
├── 📄 DOCUMENTACION-TECNICA.md     ← GUÍA COMPLETA PARA DEVS ⭐
├── 📄 GUIA-RAPIDA.md              ← Referencia rápida (START HERE!)
├── 📄 DIAGRAMAS-VISUALES.md        ← Visualizaciones y flujos
├── 📄 VERIFICACION-COMPLETA.md     ← Checklist de verificación
├── 📄 INICIO-RAPIDO.md             ← Setup inicial
│
├── 📁 Poll-it.Shared/              ← Modelos compartidos
│   └── PainPoint.cs
│
├── 📁 Poll-it.Server/              ← Backend API
│   ├── Data/
│   ├── Hubs/
│   └── Program.cs
│
└── 📁 Poll-it.Client/              ← Frontend Blazor
    ├── Pages/
    ├── Layout/
    └── wwwroot/
```

---

## 🎯 Selecciona tu Documentación Según tu Rol

### 👨‍💼 Gerente / Stakeholder
**Necesitas entender QUÉ es Poll-it**

1. Lee: [README.md](README.md) - Descripción general en 5 minutos
2. Mira: [DIAGRAMAS-VISUALES.md](DIAGRAMAS-VISUALES.md) - Diagrama arquitectónico

### 👨‍🎓 Estudiante / Aprendiz
**Necesitas aprender arquitectura de software**

1. Lee: [GUIA-RAPIDA.md](GUIA-RAPIDA.md) - Contextualización rápida
2. Lee: [ARCHITECTURE.md](ARCHITECTURE.md) - Principios SOLID explicados
3. Lee: [DIAGRAMAS-VISUALES.md](DIAGRAMAS-VISUALES.md) - Visualizaciones
4. Ejecuta: La app (ver sección abajo)

### 👨‍💻 Desarrollador Junior
**Necesitas ejecutar y entender el código**

1. [GUIA-RAPIDA.md](GUIA-RAPIDA.md) - Get it running (5 min)
2. [DOCUMENTACION-TECNICA.md](DOCUMENTACION-TECNICA.md) - Secciones principales:
   - Instalación y Configuración
   - Estructura del Proyecto
   - Componentes Principales
3. Explora el código mientras lees

### 👨‍💻 Desarrollador Senior / Arquitecto
**Necesitas entender el diseño profundamente**

1. [ARCHITECTURE.md](ARCHITECTURE.md) - Decisiones de diseño
2. [DOCUMENTACION-TECNICA.md](DOCUMENTACION-TECNICA.md) - Secciones:
   - Flujos de Trabajo
   - API RESTful
   - Guía de Desarrollo & Extensión
3. Revisa el código directamente

### 🚀 DevOps / SRE
**Necesitas deployar y mantener**

1. [DOCUMENTACION-TECNICA.md](DOCUMENTACION-TECNICA.md) - Secciones:
   - Requisitos del Sistema
   - Instalación
   - Base de Datos
   - Comandos Útiles
   - Troubleshooting

---

## 🚀 Cómo Iniciar la Aplicación

### Paso 1: Verificar Requisitos
```bash
dotnet --version
# Debe mostrar: .NET 10.0.x o superior
```

### Paso 2: Ejecutar en 2 Terminales

**Terminal 1 - Backend:**
```bash
cd Poll-it.Server
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd Poll-it.Client
dotnet run
```

**Acceso:**
- Cliente: https://localhost:3000 (o el puerto asignado)
- API: https://localhost:7001

### Paso 3: Verificar que Funciona
- Abre https://localhost:3000
- Deberías ver página de inicio
- Ve a `/submit` para enviar un "dolor"
- Ve a `/wall` para verlo en vivo

→ **Para más detalles**, ver [DOCUMENTACION-TECNICA.md - Instalación](DOCUMENTACION-TECNICA.md#instalación-y-configuración)

---

## 📱 Páginas de la Aplicación

| URL | Propósito | Descripción |
|-----|-----------|-----------|
| `/` | Home | Redirecciona a `/submit` |
| `/submit` | Formulario | Capturar "dolor" del usuario |
| `/wall` | Visualización | Panel con todos los dolores en vivo |
| `/notfound` | Error | Página 404 (automática) |

---

## 🔗 Endpoints de API

| Método | Ruta | Propósito | Detalles |
|--------|------|----------|---------|
| POST | `/api/painpoints` | Crear dolor | [Ver docs](DOCUMENTACION-TECNICA.md#endpoint-post-apipainpoints) |
| GET | `/api/painpoints` | Listar todos | [Ver docs](DOCUMENTACION-TECNICA.md#endpoint-get-apipainpoints) |
| DELETE | `/api/painpoints/{id}` | Eliminar | [Ver docs](DOCUMENTACION-TECNICA.md#endpoint-delete-apipainpointsid) |

---

## 🏗️ Stack Tecnológico

| Componente | Tecnología |
|-----------|-----------|
| Frontend | Blazor WebAssembly + Tailwind CSS |
| Backend | ASP.NET Core + Minimal API |
| Base de Datos | SQLite + Entity Framework Core |
| Tiempo Real | SignalR + WebSocket |
| Lenguaje | C# + Razor |

---

## 📊 Conceptos Clave Explicados

### 🔀 Separation of Concerns (SoC)
**¿Qué?** Dividir código por responsabilidades  
**Ejemplo en Poll-it:**
- Shared: Define qué es un PainPoint
- Server: Cómo guardarlo y notificar
- Client: Cómo mostrarlo

→ [Ver explicación completa](ARCHITECTURE.md#separation-of-concerns)

### 📡 SignalR
**¿Qué?** Comunicación en tiempo real  
**Ejemplo en Poll-it:**
- User A envía dolor → Se guarda en BD
- Server avisa a TODOS vía WebSocket
- User B ve aparecer en vivo sin refrescar

→ [Ver explicación completa](DOCUMENTACION-TECNICA.md#comunicación-en-tiempo-real-signalr)

### 🗄️ Entity Framework Core
**¿Qué?** ORM (Object-Relational Mapping)  
**Ejemplo en Poll-it:**
- Código: `painPoints.Add(newPain)`
- Genera: `INSERT INTO PainPoints VALUES (...)`

→ [Ver explicación completa](DOCUMENTACION-TECNICA.md#painpointdbcontextcs---capa-de-datos)

---

## 🐛 Problemas Comunes

### ❌ "Connection refused"
→ [Solución](DOCUMENTACION-TECNICA.md#-connection-refused-al-conectar-a-servidor)

### ❌ "CORS policy blocked"
→ [Solución](DOCUMENTACION-TECNICA.md#-cors-policy-blocked)

### ❌ "SignalR connection failed"
→ [Solución](DOCUMENTACION-TECNICA.md#-signalr-connection-failed)

**Para más**, ver [Troubleshooting completo](DOCUMENTACION-TECNICA.md#troubleshooting)

---

## 💡 Casos de Uso

### Caso 1: Ejecutar localmente
1. [Cómo Iniciar](#-cómo-iniciar-la-aplicación)
2. Abre navegador
3. Usa `/submit` y `/wall`

### Caso 2: Agregar nueva funcionalidad
1. Lee: [Guía de Desarrollo](DOCUMENTACION-TECNICA.md#guía-de-desarrollo)
2. Ejemplo: [Agregar endpoint](DOCUMENTACION-TECNICA.md#agregar-nueva-funcionalidad-ejemplo)

### Caso 3: Debugging
1. Lee: [Debugging y Logging](DOCUMENTACION-TECNICA.md#debugging-y-logging)

### Caso 4: Deployar (producción)
1. Cambiar: Strings de conexión
2. Cambiar: De SQLite a SQL Server
3. Instalar: .NET Runtime
4. Publicar: `dotnet publish -c Release`

---

## 📞 Resumen Rápido

| Quiero... | Archivo | Sección |
|-----------|---------|---------|
| Empezar rápido | GUIA-RAPIDA.md | Inicio |
| Ejecutar localmente | DOCUMENTACION-TECNICA.md | Instalación |
| Entender arquitectura | ARCHITECTURE.md | Visión General |
| Ver diagramas | DIAGRAMAS-VISUALES.md | Todos |
| Agregar código | DOCUMENTACION-TECNICA.md | Desarrollo |
| Debugging | DOCUMENTACION-TECNICA.md | Debugging |
| Problemas | DOCUMENTACION-TECNICA.md | Troubleshooting |

---

## 🎓 Ruta de Aprendizaje Recomendada

```
Día 1: Conceptualización
├─ Lee README.md (¿Qué es Poll-it?)
└─ Lee GUIA-RAPIDA.md (¿Cómo funciona?)

Día 2-3: Ejecución
├─ Ejecuta la app (DOCUMENTACION-TECNICA.md sección Instalación)
├─ Explora /submit y /wall
└─ Ve los logs del servidor

Día 4-5: Entendimiento Profundo
├─ Lee ARCHITECTURE.md (principios)
├─ Lee DIAGRAMAS-VISUALES.md (visualiza flujos)
└─ Lee DOCUMENTACION-TECNICA.md (componentes)

Día 6+: Desarrollo
├─ Modifica código
├─ Agrega features (ver guía de desarrollo)
├─ Escribe tests (si aplica)
└─ Deploy en otro entorno
```

---

## 🔗 Referencias Externas

- [Microsoft .NET Docs](https://docs.microsoft.com/dotnet)
- [Blazor Guide](https://docs.microsoft.com/aspnet/core/blazor)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [SignalR](https://docs.microsoft.com/aspnet/core/signalr)

---

## 📝 Archivos Disponibles

| Archivo | Tamaño | Propósito |
|---------|--------|---------|
| README.md | ~5 KB | Overview |
| ARCHITECTURE.md | ~30 KB | Principios |
| DOCUMENTACION-TECNICA.md | ~60 KB | **Guía Completa** ⭐ |
| GUIA-RAPIDA.md | ~8 KB | Quick Reference |
| DIAGRAMAS-VISUALES.md | ~40 KB | Visualizaciones |
| INDICE-DOCUMENTACION.md | Este archivo | Navigation |

---

## ✅ Checklist de Verificación

Después de instalar, verifica:

- [ ] `dotnet --version` muestra .NET 10.x
- [ ] `dotnet build` compila sin errores
- [ ] Server inicia: `dotnet run` (sin errores)
- [ ] Cliente inicia en otra terminal
- [ ] Página principal carga en navegador
- [ ] Puedes enviar un "dolor" desde `/submit`
- [ ] Aparece en `/wall`
- [ ] Base de datos `painpoints.db` existe

→ [Más opciones](VERIFICACION-COMPLETA.md)

---

## 🎯 Próximos Pasos

### Si eres **Usuario Final**
- Ejecuta la app: [Cómo Iniciar](#-cómo-iniciar-la-aplicación)
- Explora: `/submit` y `/wall`

### Si eres **Desarrollador**
- Ejecuta la app
- Lee: DOCUMENTACION-TECNICA.md
- Modifica código en Pages/
- Agrega nuevas funcionalidades

### Si eres **Arquitecto**
- Lee: ARCHITECTURE.md
- Lee: DOCUMENTACION-TECNICA.md
- Revisa: DIAGRAMAS-VISUALES.md
- Evalúa patrones usados

---

## 📞 Contacto / Soporte

Para dudas:
1. Consulta [Troubleshooting](DOCUMENTACION-TECNICA.md#troubleshooting)
2. Revisa [FAQ implícito](DOCUMENTACION-TECNICA.md)
3. Examina los logs

---

**Última actualización**: Marzo 5, 2025  
**Versión**: 1.0  
**Estado**: ✅ Completo

### 🌟 Recomendación Principal

Si no sabes por dónde empezar: **[→ Lee GUIA-RAPIDA.md](GUIA-RAPIDA.md)**

