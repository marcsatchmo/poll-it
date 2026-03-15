# Resolución: Error "64-bit configuration requires a basic or higher App Service plan"

## 📋 Diagnóstico Completo

### Qué significa este error

Azure App Service ha detectado una **incompatibilidad de configuración**:

- **Situación actual**: Tu App Service está en el plan **Free (F1)** o **Shared (D1)**
- **Configuración incompatible**: Tienes activada la plataforma **64 bits** en General Settings
- **Problema**: Los planes Free y Shared **solo soportan 32 bits**
- **Requisito**: La configuración de 64 bits necesita un plan **Basic (B1) o superior**

---

## 🔍 Análisis del Proyecto

### Configuración del Proyecto (Poll-it.Server.csproj)

```xml
<TargetFramework>net10.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

✅ **Conclusión**: El proyecto está correctamente configurado para .NET 10.0 y es agnóstico a la arquitectura (32 o 64 bits). No hay nada en el proyecto que fuerce 64 bits.

### Workflows de GitHub Actions

El archivo `.github/workflows/deploy-backend.yml` **NO contiene** configuraciones que fuercen arquitectura de 64 bits. El deployment es automático y sigue las configuraciones del App Service en Azure Portal.

### Program.cs y appsettings.Production.json

✅ **Ambos archivos están correctamente configurados** y no tienen referencias a arquitectura de 64 bits.

---

## ✅ Soluciones Disponibles

### OPCIÓN A: Cambiar a Plan Basic (B1) o Superior ⭐ **RECOMENDADO PARA PRODUCCIÓN**

**Ventajas:**
- Soporta 64 bits nativo
- Mejor rendimiento
- SLA garantizado (99.95%)
- Escalado automático disponible

**Ventajas de 64 bits:**
- Mejor manejo de memoria para aplicaciones grandes
- Mayor caché
- Mejor rendimiento en operaciones numéricas

**Pasos en Azure Portal:**

1. **Abre Azure Portal** → Busca tu App Service (`poll-it-abe8chfgghe9gyb7`)

2. **Ve a Settings → Scale Up (App Service Plan)**

3. **Selecciona el plan Basic (B1)** o superior:
   ```
   Basic (B1) - Recomendado
   └─ Precio: ~$0.05/hora
   └─ 1 vCPU, 1.75 GB RAM
   └─ Soporta 64 bits nativo
   ```

4. **Haz clic en Select** → Confirma el cambio

5. **Espera 2-5 minutos** mientras Azure redimensiona tu App Service

6. **Verifica que el cambio se aplicó:**
   - Ve a Settings → General Settings
   - Verifica que Platform ahora muestre **"64 Bit"** sin errores

---

### OPCIÓN B: Cambiar a 32 bits ⭐ **PARA DESARROLLO/DEMOSTRACIÓN**

**Ventajas:**
- Sigue usando el plan Free (F1)
- Sin costos adicionales
- Suficiente para aplicaciones de demostración

**Limitaciones:**
- Máximo 1 GB de RAM
- Sin SLA
- Limitado a 60 minutos de ejecución diaria (plan Free)
- No recomendado para producción real

**Pasos en Azure Portal:**

1. **Abre Azure Portal** → Tu App Service

2. **Ve a Settings → General Settings**

3. **En la sección "Platform":**
   - Encuentra la opción **"64 Bit"** (actualmente activada ✓)
   - Haz clic para desactivarla (cambia a ✗)

4. **Haz clic en Save** en la parte superior

5. **Espera 1-2 minutos** para que se aplique el cambio

6. **Verifica que ya no haya error** y que muestre "32 Bit"

---

## 🎯 Recomendación Final

### Para Poll-it (Aplicación de Demostración en Azure)

**Te recomiendo: OPCIÓN A - Plan Basic (B1)**

**Razones:**
1. Tu aplicación está en **producción pública** (Azure Static Web Apps + App Service)
2. CORS y SignalR funcionan mejor con 64 bits
3. Mejor rendimiento general
4. Plan Basic es muy accesible economicamente (~$0.05/hora)
5. Si en el futuro necesitas escalar, ya tienes la infraestructura soportada

---

## 📝 Checklist de Aplicación

### Si eliges OPCIÓN A (Basic B1):

- [ ] Accedí a Azure Portal
- [ ] Fui a App Service → Scale Up
- [ ] Seleccioné plan Basic (B1)
- [ ] Confirmé el cambio
- [ ] Esperé 2-5 minutos a que se aplique
- [ ] Verifiqué en General Settings que ahora muestra "64 Bit" sin error
- [ ] Testé el backend en Azure (hice una petición a `/api/painpoints`)
- [ ] Verifiqué que el frontend en Static Web Apps sigue funcionando

### Si eliges OPCIÓN B (32 bits):

- [ ] Accedí a Azure Portal
- [ ] Fui a Settings → General Settings
- [ ] Desactivé la opción "64 Bit" (cambié a ✗)
- [ ] Hice clic en Save
- [ ] Esperé 1-2 minutos
- [ ] Verifiqué que el error desapareció
- [ ] Testé que todo funciona correctamente

---

## 🔧 Verificación Post-Cambio

**En todos los casos, verifica que:**

1. **Azure Portal - General Settings**
   - ✅ No hay error rojo sobre "64-bit configuration requires..."
   - ✅ La plataforma muestra correctamente "32 Bit" o "64 Bit"

2. **Desde el Navegador - Frontend**
   - ✅ Abre https://orange-moss-00032b10f.1.azurestaticapps.net
   - ✅ Intenta crear un "pain point"
   - ✅ Verifica en DevTools → Network que la petición a `/api/painpoints` devuelve 201 (creada) o 200 (get)

3. **Azure Portal - App Service Logs**
   - Ve a Settings → App Service logs
   - Filtra por errores recientes
   - No debe haber errores de arquitectura

---

## ❓ Preguntas Frecuentes

### ¿Por qué Azure activó 64 bits automáticamente?

Algunos runtimes/frameworks por defecto usan 64 bits. Azure puede haber detectado .NET 10.0 y activarlo automáticamente, pero tu plan no lo soporta.

### ¿Afecta esto al frontend (Static Web Apps)?

**No**, el frontend está en Azure Static Web Apps (servicio diferente) y no tiene esta limitación.

### ¿Puedo cambiar de plan después sin problemas?

**Sí**, es una operación reversible y segura. Si cambias a Basic y luego quieres volver a Free, puedes hacerlo (aunque perderías la capacidad de 64 bits).

### ¿Cuál es el costo del plan Basic?

Aproximadamente **$0.05/hora** (varía por región). Consúltalo en Azure Portal → App Service Plan → Pricing Tier.

### ¿Necesito hacer re-deploy después del cambio?

**No**, el cambio de plan es inmediato en Azure Portal. Tu código ya está deployado. Solo necesitas que la plataforma cambie de configuración.

---

## 📞 Soporte Adicional

Si después de aplicar esta solución sigues viendo errores:

1. **Verifica en Azure Portal:**
   - App Service → Logs stream
   - Busca mensajes de error recientes

2. **Reinicia el App Service:**
   - Ve a Overview → Restart

3. **Revisa la compatibilidad de .NET:**
   - .NET 10.0 requiere .NET runtime actualizado en el plan seleccionado
   - Los planes Basic y superiores siempre están actualizados

---

## Referencias

- [Azure App Service Plans - Límites y capacidades](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans)
- [Cambiar el plan de App Service](https://learn.microsoft.com/en-us/azure/app-service/manage-scale-up)
- [Platform Settings en Azure Portal](https://learn.microsoft.com/en-us/azure/app-service/configure-common?tabs=portal)
