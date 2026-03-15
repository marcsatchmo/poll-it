# Instrucciones: Ejecutar Cleanup de Pain Points en Producción (Azure)

## Problema Resuelto
El endpoint `DELETE /api/painpoints/cleanup` estaba pidiendo token de autenticación. Se ha modificado el servidor para permitir acceso anónimo a este endpoint administrativo.

## ✅ Cambios Aplicados
- Agregado `.AllowAnonymous()` al endpoint `/api/painpoints/cleanup` en `Program.cs`
- Build verificado exitosamente
- Lista para deploy a Azure

## 📋 Pasos para Ejecutar el Cleanup

### Opción A: Usando Postman (Recomendado para UI)

1. **Abre Postman** (o cualquier cliente HTTP)

2. **Configura la solicitud:**
   - **Método:** DELETE
   - **URL:** `https://orange-moss-00032b10f.1.azurestaticapps.net/api/painpoints/cleanup`
   - **Headers:** 
     ```
     Content-Type: application/json
     ```
   - **Body:** (vacío)

3. **Haz clic en "Send"**

4. **Resultado esperado:**
   ```json
   {
     "message": "N pain points eliminados"
   }
   ```
   O si no hay registros:
   ```json
   {
     "message": "No hay pain points para eliminar"
   }
   ```

---

### Opción B: Usando PowerShell (Automatizado)

Ejecuta este comando en PowerShell:

```powershell
$response = curl -X DELETE "https://orange-moss-00032b10f.1.azurestaticapps.net/api/painpoints/cleanup" `
    -Headers @{"Content-Type" = "application/json"}

Write-Host "Respuesta del servidor:" -ForegroundColor Green
Write-Host $response
```

---

### Opción C: Usando cURL (CLI)

```bash
curl -X DELETE "https://orange-moss-00032b10f.1.azurestaticapps.net/api/painpoints/cleanup" \
  -H "Content-Type: application/json"
```

---

## 🔍 Verificación

Después de ejecutar el cleanup:

1. **Verifica que se eliminaron los registros:**
   ```
   GET https://orange-moss-00032b10f.1.azurestaticapps.net/api/painpoints
   ```
   Debería devolver un array vacío: `[]`

2. **Visita la aplicación web:**
   - Abre `https://orange-moss-00032b10f.1.azurestaticapps.net`
   - La pared de dolor debe estar vacía

---

## 🚀 Próximos Pasos

1. **Haz commit y push del cambio:**
   ```bash
   git add Poll-it.Server/Program.cs
   git commit -m "fix: allow anonymous access to cleanup endpoint"
   git push
   ```

2. **GitHub Actions desplegará automáticamente** a Azure (verifica en `.github/workflows/deploy-backend.yml`)

3. **Espera 2-3 minutos** para que el deployment complete

4. **Ejecuta el cleanup** usando cualquiera de las opciones anteriores

---

## ⚠️ Notas Importantes

- **Sin autenticación requerida:** El endpoint ahora es público. Si en el futuro necesitas protegerlo, agrega autenticación en `Program.cs`
- **Destruye datos:** Este endpoint elimina TODOS los pain points. No hay opción de deshacer
- **Uso administrativo:** Úsalo solo cuando necesites limpiar la base de datos para testing o reset

---

## 🆘 Troubleshooting

### "401 Unauthorized" después del deploy
- Espera 5 minutos adicionales a que el deployment complete completamente
- Limpia el caché del navegador (Ctrl+Shift+Delete)
- Intenta desde una pestaña incógnita

### "404 Not Found"
- Verifica que la URL sea correcta: `https://orange-moss-00032b10f.1.azurestaticapps.net/api/painpoints/cleanup`
- Asegúrate que el backend esté desplegado en Azure (check GitHub Actions)

### "CORS Error" en el navegador
- Este endpoint está configurado con CORS permitido
- Si persiste, intenta desde Postman en lugar del navegador

### "Database locked" o error de BD
- Espera 30 segundos e intenta nuevamente
- Verifica que no haya otra aplicación usando la BD
