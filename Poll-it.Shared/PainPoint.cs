namespace Poll_it.Shared;

/// <summary>
/// Representa un "punto de dolor" o frustración técnica expresada por un analista funcional.
/// Este modelo es compartido entre el cliente y el servidor para mantener consistencia en toda la aplicación.
/// </summary>
/// <remarks>
/// Arquitectura: Este es nuestro modelo de dominio central. Al estar en el proyecto Shared,
/// garantizamos que tanto el cliente (Blazor) como el servidor (API) trabajen con la misma
/// estructura de datos, evitando duplicación y posibles inconsistencias.
/// </remarks>
public class PainPoint
{
    /// <summary>
    /// Identificador único del punto de dolor.
    /// </summary>
    /// <remarks>
    /// Se genera automáticamente en el servidor al crear un nuevo registro.
    /// </remarks>
    public int Id { get; set; }

    /// <summary>
    /// El texto que describe el dolor o frustración técnica.
    /// </summary>
    /// <remarks>
    /// Este es el contenido principal que el usuario ingresa y que se mostrará
    /// en la pantalla de resultados como una nota tipo Post-it.
    /// </remarks>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en que se creó el punto de dolor.
    /// </summary>
    /// <remarks>
    /// Se registra automáticamente en el servidor al guardar el registro.
    /// Útil para ordenar y mostrar los puntos de dolor cronológicamente.
    /// </remarks>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Color asignado al Post-it para visualización.
    /// </summary>
    /// <remarks>
    /// Se asigna aleatoriamente al crear el registro para dar variedad visual
    /// y hacer la interfaz más atractiva y dinámica.
    /// Ejemplos: "yellow", "pink", "blue", "green", etc.
    /// </remarks>
    public string Color { get; set; } = "yellow";
}
