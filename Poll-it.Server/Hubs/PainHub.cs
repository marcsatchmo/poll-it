using Microsoft.AspNetCore.SignalR;
using Poll_it.Shared;

namespace Poll_it.Server.Hubs;

/// <summary>
/// Hub de SignalR para la comunicación en tiempo real de puntos de dolor.
/// Permite que todos los clientes conectados reciban actualizaciones instantáneas.
/// </summary>
/// <remarks>
/// Arquitectura - Comunicación en Tiempo Real:
/// 
/// SignalR es una biblioteca que simplifica la adición de funcionalidad web en tiempo real
/// a las aplicaciones. Un "Hub" es el centro de comunicación que:
/// 
/// 1. GESTIONA CONEXIONES:
///    - Mantiene un registro de todos los clientes conectados
///    - Maneja conexiones y desconexiones automáticamente
///    - Asigna un ConnectionId único a cada cliente
/// 
/// 2. TRANSMITE MENSAJES:
///    - Puede enviar mensajes a todos los clientes (broadcast)
///    - Puede enviar a clientes específicos o grupos
///    - Usa WebSockets cuando es posible, con fallback automático a otras técnicas
/// 
/// 3. ABSTRAE LA COMPLEJIDAD:
///    - No necesitas manejar manualmente sockets
///    - La reconexión automática está incorporada
///    - Serialización/deserialización automática de objetos
/// 
/// En Poll-it:
/// - Cuando un usuario envía un "dolor", el servidor lo guarda en la BD
/// - Luego usa este Hub para notificar INSTANTÁNEAMENTE a todos los demás usuarios
/// - Cada usuario ve aparecer el nuevo Post-it sin refrescar la página
/// 
/// Esto ejemplifica el patrón "Pub/Sub" (Publicador/Suscriptor):
/// - El servidor publica eventos
/// - Los clientes se suscriben para recibirlos
/// - Separación de concerns: el Hub solo maneja la comunicación, no la lógica de negocio
/// </remarks>
public class PainHub : Hub
{
    /// <summary>
    /// Se llama automáticamente cuando un cliente se conecta al Hub.
    /// </summary>
    /// <remarks>
    /// Este método es útil para:
    /// - Logging de conexiones
    /// - Agregar el cliente a grupos específicos
    /// - Inicializar el estado de la conexión
    /// 
    /// En producción, aquí podrías implementar autenticación adicional.
    /// </remarks>
    public override async Task OnConnectedAsync()
    {
        // Obtener información sobre el cliente conectado
        var connectionId = Context.ConnectionId;
        
        // Log para debugging (en producción usarías ILogger)
        Console.WriteLine($"[PainHub] Cliente conectado: {connectionId}");
        
        // Llamar al método base que maneja la infraestructura de SignalR
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Se llama automáticamente cuando un cliente se desconecta del Hub.
    /// </summary>
    /// <param name="exception">Excepción si la desconexión fue por error, null si fue normal</param>
    /// <remarks>
    /// Útil para:
    /// - Limpieza de recursos
    /// - Actualizar el estado de "usuarios activos"
    /// - Logging de desconexiones
    /// 
    /// SignalR maneja automáticamente la reconexión si la desconexión fue temporal.
    /// </remarks>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        
        if (exception != null)
        {
            Console.WriteLine($"[PainHub] Cliente desconectado con error: {connectionId} - {exception.Message}");
        }
        else
        {
            Console.WriteLine($"[PainHub] Cliente desconectado: {connectionId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Método opcional: Los clientes pueden llamar este método para enviar un mensaje a todos.
    /// </summary>
    /// <param name="painPoint">El punto de dolor a transmitir</param>
    /// <remarks>
    /// Este es un método de ejemplo que muestra cómo los clientes pueden invocar métodos del Hub.
    /// 
    /// En nuestra implementación actual, NO usamos este método directamente.
    /// En su lugar:
    /// 1. El cliente envía el dolor via POST HTTP al endpoint /api/painpoints
    /// 2. El endpoint guarda en la BD y usa IHubContext para notificar
    /// 
    /// Esta aproximación es mejor porque:
    /// - Centraliza la lógica de negocio en los endpoints HTTP
    /// - El Hub queda como pura infraestructura de comunicación
    /// - Más fácil de testear y mantener
    /// 
    /// Sin embargo, este método sirve como ejemplo de cómo funcionan las invocaciones
    /// cliente-a-servidor en SignalR.
    /// </remarks>
    public async Task SendPainPoint(PainPoint painPoint)
    {
        // Transmitir el punto de dolor a TODOS los clientes conectados
        // "ReceivePainPoint" es el nombre del método que los clientes deben implementar
        await Clients.All.SendAsync("ReceivePainPoint", painPoint);
    }

    /// <summary>
    /// Método opcional: Enviar un punto de dolor solo a un cliente específico.
    /// </summary>
    /// <param name="connectionId">ID de conexión del cliente destinatario</param>
    /// <param name="painPoint">El punto de dolor a enviar</param>
    /// <remarks>
    /// Ejemplo de comunicación punto-a-punto.
    /// Útil si en el futuro quisieras:
    /// - Notificaciones privadas
    /// - Feedback específico al remitente
    /// - Mensajes de validación personalizados
    /// </remarks>
    public async Task SendPainPointToClient(string connectionId, PainPoint painPoint)
    {
        await Clients.Client(connectionId).SendAsync("ReceivePainPoint", painPoint);
    }

    /// <summary>
    /// Método opcional: Enviar un punto de dolor a todos EXCEPTO al remitente.
    /// </summary>
    /// <param name="painPoint">El punto de dolor a transmitir</param>
    /// <remarks>
    /// Útil cuando:
    /// - El remitente ya tiene el dato localmente
    /// - Quieres evitar duplicación en el cliente que envió
    /// - Necesitas diferentes comportamientos para el remitente vs otros clientes
    /// </remarks>
    public async Task SendPainPointToOthers(PainPoint painPoint)
    {
        // Clients.Others envía a todos excepto al que invocó el método
        await Clients.Others.SendAsync("ReceivePainPoint", painPoint);
    }
}
