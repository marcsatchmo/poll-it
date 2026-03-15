# Script para limpiar los pain points de la base de datos
# Este script ejecuta el endpoint DELETE /api/painpoints/cleanup

Write-Host "[CLEANUP] Iniciando limpieza de pain points..." -ForegroundColor Yellow

# Ejecutar curl para llamar al endpoint de limpieza en localhost
$response = curl -X DELETE "http://localhost:5048/api/painpoints/cleanup" `
    -Headers @{"Content-Type" = "application/json"} `
    -ErrorAction SilentlyContinue

Write-Host "[CLEANUP] Respuesta del servidor:" -ForegroundColor Green
Write-Host $response

Write-Host "[CLEANUP] Limpieza completada" -ForegroundColor Green
