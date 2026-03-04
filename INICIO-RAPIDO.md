# 🚀 Inicio Rápido - Poll-it

## Ejecutar la Aplicación en 3 Pasos

### Paso 1: Iniciar el Servidor (Backend)

Abre una terminal en la raíz del proyecto y ejecuta:

```powershell
cd Poll-it.Server
dotnet run --launch-profile https
```

✅ **Espera ver:** `Now listening on: https://localhost:7001`

El servidor estará listo cuando veas el mensaje de inicialización de la base de datos.

---

### Paso 2: Iniciar el Cliente (Frontend)

**Sin cerrar la terminal anterior**, abre una **nueva terminal** y ejecuta:

```powershell
cd Poll-it.Client
dotnet run --launch-profile https
```

✅ **Espera ver:** `Now listening on: https://localhost:7219`

El navegador se abrirá automáticamente.

---

### Paso 3: Usar la Aplicación

1. **Página de Inicio (`https://localhost:7219`)**: Te redirige automáticamente a `/submit`
2. **Enviar un dolor**: Escribe tu frustración técnica y haz click en "Enviar"
3. **Ver el muro**: Navega a `/wall` o haz click en "Ver el Muro de Dolores"

---

## 🎯 URLs Importantes

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Cliente** | https://localhost:7219 | Aplicación Blazor (Frontend) |
| **Servidor** | https://localhost:7001 | API + SignalR Hub (Backend) |
| **API Docs** | https://localhost:7001/swagger | Documentación de la API (si está habilitado) |

---

## ⚡ Atajos

### Verificar que todo funciona

1. Abre `https://localhost:7219/submit`
2. Escribe "Test de conexión"
3. Haz click en "Enviar mi Dolor"
4. Abre `https://localhost:7219/wall` en otra pestaña
5. Deberías ver tu dolor aparecer en el muro

### Ver logs del servidor

Los logs aparecen en la terminal donde ejecutaste el servidor. Busca:
- `✅ Base de datos inicializada correctamente`
- Cada vez que alguien envía un dolor verás: `POST /api/painpoints`
- Conexiones SignalR: `SignalR connection established`

---

## 🐛 Solución Rápida de Problemas

### El servidor no inicia

```powershell
# Verifica que el puerto 7001 no esté en uso
netstat -ano | findstr :7001

# Si está en uso, mata el proceso
taskkill /PID <número_de_proceso> /F
```

### El cliente no se conecta

1. Asegúrate de que el **servidor esté corriendo primero**
2. Verifica que veas `https://localhost:7001` en los logs del servidor
3. Presiona F12 en el navegador y busca errores en la consola

### Error de certificado HTTPS

```powershell
# Confía en el certificado de desarrollo
dotnet dev-certs https --trust
```

---

## 📝 Notas Importantes

- ⚠️ **SIEMPRE inicia el servidor ANTES que el cliente**
- ⚠️ **NO cierres las terminales** mientras uses la aplicación
- ⚠️ La base de datos SQLite se crea automáticamente en `Poll-it.Server/painpoints.db`
- ⚠️ Los cambios en el código requieren reiniciar (Ctrl+C y volver a ejecutar)

---

## 🎓 Para Demostración en Clase

### Configuración Ideal

1. **Proyector/Pantalla Grande**: Muestra `https://localhost:7219/wall`
2. **Estudiantes**: Acceden a `https://localhost:7219/submit` desde sus dispositivos
3. **Resultado**: Todos ven los dolores aparecer en tiempo real en la pantalla grande

### Conectar dispositivos externos (mismo WiFi)

1. Encuentra tu IP local:
   ```powershell
   ipconfig
   # Busca "Dirección IPv4"
   ```

2. Modifica `Poll-it.Server/Properties/launchSettings.json`:
   - Cambia `applicationUrl` a: `https://0.0.0.0:7001;http://0.0.0.0:5000`

3. Actualiza CORS en `Poll-it.Server/Program.cs`:
   ```csharp
   policy.AllowAnyOrigin()  // En lugar de orígenes específicos
   ```

4. Los estudiantes acceden a: `https://TU_IP:7219`

---

**¿Listo? ¡Ejecuta los pasos 1 y 2 y comienza a capturar dolores técnicos! 🎉**
