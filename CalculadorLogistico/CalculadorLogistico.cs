namespace CalculadorLogistico;

public class CalculadorLogistico
{
    /// <summary>
    /// Calculates the shipping cost between two geographic zones with advanced logistics.
    /// </summary>
    /// <param name="cantidadKilogramos">The weight in kilograms</param>
    /// <param name="codigoZonaOrigen">The origin zone code</param>
    /// <param name="codigoZonaDestino">The destination zone code</param>
    /// <param name="tarifasBasePorTrayecto">Dictionary with (origin, destination) tuple as key and base rate as value</param>
    /// <param name="log">Output parameter containing the processing log in Spanish</param>
    /// <returns>The total shipping cost rounded to 2 decimals</returns>
    /// <exception cref="ZonaDesconocidaException">Thrown when zones are not found in any route</exception>
    public decimal CalcularTarifaEnvio(
        decimal cantidadKilogramos,
        string codigoZonaOrigen,
        string codigoZonaDestino,
        Dictionary<(string, string), decimal> tarifasBasePorTrayecto,
        out string log)
    {
        // Validate zones exist in the dictionary
        ValidarZonas(codigoZonaOrigen, codigoZonaDestino, tarifasBasePorTrayecto);

        var fechaActual = DateTime.Now;
        var claveDirecta = (codigoZonaOrigen, codigoZonaDestino);

        // 1. Try direct route
        if (tarifasBasePorTrayecto.TryGetValue(claveDirecta, out var tarifaDirecta))
        {
            var costoTotal = Math.Round(cantidadKilogramos * tarifaDirecta, 2);
            log = $"En la fecha {fechaActual:dd-MM-yyyy HH:mm:ss} se procesó un envío de {cantidadKilogramos} kg desde {codigoZonaOrigen} hacia {codigoZonaDestino}. Costo total calculado: {costoTotal}.";
            return costoTotal;
        }

        // 2. Try reverse route
        var claveReversa = (codigoZonaDestino, codigoZonaOrigen);
        if (tarifasBasePorTrayecto.TryGetValue(claveReversa, out var tarifaReversa))
        {
            var costoBase = cantidadKilogramos * tarifaReversa;
            var recargo = costoBase * 0.10m;
            var costoTotal = Math.Round(costoBase + recargo, 2);

            log = $"En la fecha {fechaActual:dd-MM-yyyy HH:mm:ss} se procesó un envío de {cantidadKilogramos} kg desde {codigoZonaOrigen} hacia {codigoZonaDestino}. " +
                  $"No se encontró ruta directa. Se utilizó ruta inversa con tarifa base {tarifaReversa}. " +
                  $"Monto base: {costoBase}, recargo 10%: {recargo}, costo total calculado: {costoTotal}.";
            return costoTotal;
        }

        // 3. Try transshipment
        var resultadoTransbordo = CalcularTransbordo(cantidadKilogramos, codigoZonaOrigen, codigoZonaDestino, tarifasBasePorTrayecto);
        if (resultadoTransbordo.HasValue)
        {
            var (costoTotal, logTransbordo) = resultadoTransbordo.Value;
            log = $"En la fecha {fechaActual:dd-MM-yyyy HH:mm:ss} se procesó un envío de {cantidadKilogramos} kg desde {codigoZonaOrigen} hacia {codigoZonaDestino}. {logTransbordo}";
            return costoTotal;
        }

        // 4. Not processed
        log = $"En la fecha {fechaActual:dd-MM-yyyy HH:mm:ss} NO se procesó el envío de {cantidadKilogramos} kg desde {codigoZonaOrigen} hacia {codigoZonaDestino}. No se encontró ninguna ruta disponible.";
        return 0m;
    }

    /// <summary>
    /// Validates that both zones exist in at least one route in the dictionary.
    /// </summary>
    private void ValidarZonas(string zonaOrigen, string zonaDestino, Dictionary<(string, string), decimal> tarifas)
    {
        bool zonaOrigenExiste = tarifas.Keys.Any(k => k.Item1 == zonaOrigen || k.Item2 == zonaOrigen);
        bool zonaDestinoExiste = tarifas.Keys.Any(k => k.Item1 == zonaDestino || k.Item2 == zonaDestino);

        if (!zonaOrigenExiste && !zonaDestinoExiste)
        {
            throw new ZonaDesconocidaException(zonaOrigen, zonaDestino);
        }
        else if (!zonaOrigenExiste)
        {
            throw new ZonaDesconocidaException(zonaOrigen);
        }
        else if (!zonaDestinoExiste)
        {
            throw new ZonaDesconocidaException(zonaDestino);
        }
    }

    /// <summary>
    /// Calculates transshipment cost by finding an intermediate route.
    /// </summary>
    private (decimal costoTotal, string log)? CalcularTransbordo(
        decimal cantidadKilogramos,
        string origen,
        string destino,
        Dictionary<(string, string), decimal> tarifas)
    {
        // Find first intermediate zone B where (A,B) and (B,C) exist
        foreach (var ruta in tarifas.Keys)
        {
            if (ruta.Item1 == origen && tarifas.ContainsKey((ruta.Item2, destino)))
            {
                var zonaIntermedia = ruta.Item2;
                var tarifaPrimerTramo = tarifas[ruta];
                var tarifaSegundoTramo = tarifas[(zonaIntermedia, destino)];

                var costoPrimerTramo = cantidadKilogramos * tarifaPrimerTramo;
                var costoSegundoTramo = cantidadKilogramos * tarifaSegundoTramo;
                var costoTotal = Math.Round(costoPrimerTramo + costoSegundoTramo, 2);

                var log = $"Se utilizó transbordo a través de {zonaIntermedia}. " +
                         $"Primer tramo ({origen}→{zonaIntermedia}): {costoPrimerTramo}, " +
                         $"segundo tramo ({zonaIntermedia}→{destino}): {costoSegundoTramo}, " +
                         $"costo total calculado: {costoTotal}.";

                return (costoTotal, log);
            }
        }

        return null;
    }
}
