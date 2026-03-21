namespace CalculadorLogistico;

/// <summary>
/// Exception thrown when a shipping zone is not found in any route.
/// </summary>
public class ZonaDesconocidaException : Exception
{
    public ZonaDesconocidaException(string zona)
        : base($"La zona '{zona}' no existe en ninguna ruta disponible.")
    {
    }

    public ZonaDesconocidaException(string zonaOrigen, string zonaDestino)
        : base($"Las zonas '{zonaOrigen}' y '{zonaDestino}' no existen en ninguna ruta disponible.")
    {
    }
}
