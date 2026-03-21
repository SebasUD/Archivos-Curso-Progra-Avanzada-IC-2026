namespace CalculadorLogistico;

public class CalculadorLogistico
{
    /// <summary>
    /// Calculates the shipping cost between two geographic zones.
    /// </summary>
    /// <param name="cantidadKilogramos">The weight in kilograms</param>
    /// <param name="codigoZonaOrigen">The origin zone code</param>
    /// <param name="codigoZonaDestino">The destination zone code</param>
    /// <param name="tarifasBasePorTrayecto">Dictionary with (origin, destination) tuple as key and base rate as value</param>
    /// <returns>The total shipping cost (weight * base rate)</returns>
    public decimal CalcularTarifaEnvio(
        decimal cantidadKilogramos,
        string codigoZonaOrigen,
        string codigoZonaDestino,
        Dictionary<(string, string), decimal> tarifasBasePorTrayecto)
    {
        var clave = (codigoZonaOrigen, codigoZonaDestino);
        
        if (tarifasBasePorTrayecto.TryGetValue(clave, out var tarifaBase))
        {
            return cantidadKilogramos * tarifaBase;
        }

        return 0m;
    }
}
