using Microsoft.EntityFrameworkCore;
using Poll_it.Server.Data;

namespace Poll_it.Server;

/// <summary>
/// Script temporal para limpiar los pain points de la base de datos.
/// Este archivo se debe ejecutar una sola vez y luego ser eliminado.
/// </summary>
public class CleanupScript
{
    public static async Task DeleteAllPainPoints(PainPointDbContext db)
    {
        try
        {
            // Obtener todos los pain points
            var painPoints = await db.PainPoints.ToListAsync();
            
            if (painPoints.Count > 0)
            {
                Console.WriteLine($"[CLEANUP] Encontrados {painPoints.Count} pain points para eliminar...");
                
                // Eliminar todos
                db.PainPoints.RemoveRange(painPoints);
                await db.SaveChangesAsync();
                
                Console.WriteLine($"[CLEANUP] ✓ {painPoints.Count} pain points eliminados exitosamente");
            }
            else
            {
                Console.WriteLine("[CLEANUP] No hay pain points para eliminar");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLEANUP] ✗ Error al eliminar pain points: {ex.Message}");
            throw;
        }
    }
}
