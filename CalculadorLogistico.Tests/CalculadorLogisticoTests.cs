using System.Collections.Generic;
using Xunit;
using CalculadorLogistico;

namespace CalculadorLogistico.Tests;

public class CalculadorLogisticoTests
{
    private readonly global::CalculadorLogistico.CalculadorLogistico _calculador = new();

    #region Direct Route Tests

    [Fact]
    void CalcularTarifaEnvio_WithDirectRoute_ReturnsCorrectTotal()
    {
        // Arrange
        decimal kilos = 10m;
        string zonaOrigen = "A";
        string zonaDestino = "B";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(50m, resultado);
    }

    [Fact]
    void CalcularTarifaEnvio_WithSmallWeight_ReturnsCorrectTotal()
    {
        // Arrange
        decimal kilos = 1m;
        string zonaOrigen = "Z1";
        string zonaDestino = "Z2";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("Z1", "Z2"), 15.5m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(15.5m, resultado);
    }

    [Fact]
    void CalcularTarifaEnvio_WithLargeWeight_ReturnsCorrectTotal()
    {
        // Arrange
        decimal kilos = 1000m;
        string zonaOrigen = "NORTH";
        string zonaDestino = "SOUTH";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("NORTH", "SOUTH"), 2.75m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(2750m, resultado);
    }

    [Fact]
    void CalcularTarifaEnvio_WithDecimalWeight_ReturnsCorrectTotal()
    {
        // Arrange
        decimal kilos = 5.5m;
        string zonaOrigen = "EAST";
        string zonaDestino = "WEST";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("EAST", "WEST"), 12.25m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(67.38m, resultado); // Rounded to 2 decimals
    }

    [Fact]
    void CalcularTarifaEnvio_WithZeroWeight_ReturnsZero()
    {
        // Arrange
        decimal kilos = 0m;
        string zonaOrigen = "X";
        string zonaDestino = "Y";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("X", "Y"), 10m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(0m, resultado);
    }

    [Fact]
    void CalcularTarifaEnvio_WithMultipleRoutes_SelectsCorrectRoute()
    {
        // Arrange
        decimal kilos = 20m;
        string zonaOrigen = "A";
        string zonaDestino = "B";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 8m },
            { ("A", "C"), 10m },
            { ("B", "C"), 6m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(160m, resultado);
    }

    [Fact]
    void CalcularTarifaEnvio_UnknownZones_ThrowsException()
    {
        // Arrange
        decimal kilos = 15m;
        string zonaOrigen = "X";
        string zonaDestino = "Y";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m },
            { ("B", "C"), 3m }
        };

        // Act & Assert
        var exception = Assert.Throws<ZonaDesconocidaException>(() =>
            _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _));

        Assert.Contains("no existen en ninguna ruta disponible", exception.Message);
    }

    [Fact]
    void CalcularTarifaEnvio_RouteNotFound_ReturnsZero()
    {
        // Arrange
        decimal kilos = 15m;
        string zonaOrigen = "A";
        string zonaDestino = "D";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m },
            { ("B", "C"), 3m },
            { ("D", "E"), 7m }
            // A and D exist, but no connection: no direct A->D, no reverse D->A, no transshipment possible
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out _);

        // Assert
        Assert.Equal(0m, resultado);
    #endregion

    #region Log Format Tests

    //[Fact]
    void CalcularTarifaEnvio_DirectRoute_LogFormatIsCorrect()
    {
        // Arrange
        decimal kilos = 10m;
        string zonaOrigen = "A";
        string zonaDestino = "B";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        Assert.Contains("En la fecha", log);
        Assert.Contains("se procesó un envío de 10 kg desde A hacia B", log);
        Assert.Contains("Costo total calculado: 50", log);
        // Validate date format dd-mm-yyyy hh24:mi:ss
        Assert.Matches(@"\d{2}-\d{2}-\d{4} \d{2}:\d{2}:\d{2}", log);
    }

    //[Fact]
    void CalcularTarifaEnvio_NoRouteFound_LogIndicatesNotProcessed()
    {
        // Arrange
        decimal kilos = 15m;
        string zonaOrigen = "A";
        string zonaDestino = "D";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m },
            { ("B", "C"), 3m },
            { ("D", "E"), 7m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        Assert.Contains("NO se procesó", log);
        Assert.Contains("15 kg desde A hacia D", log);
        Assert.Contains("No se encontró ninguna ruta disponible", log);
    }

    #endregion

    #region Reverse Route Tests

    //[Fact]
    void CalcularTarifaEnvio_ReverseRoute_AppliesTenPercentSurcharge()
    {
        // Arrange
        decimal kilos = 20m;
        string zonaOrigen = "B";
        string zonaDestino = "A";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m } // Only A->B exists, so B->A will use reverse
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        // Base cost: 20 * 5 = 100
        // Surcharge: 100 * 0.10 = 10
        // Total: 100 + 10 = 110
        Assert.Equal(110m, resultado);
    }

    //[Fact]
    void CalcularTarifaEnvio_ReverseRoute_LogExplainsCalculation()
    {
        // Arrange
        decimal kilos = 10m;
        string zonaOrigen = "B";
        string zonaDestino = "A";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 8m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        Assert.Contains("No se encontró ruta directa", log);
        Assert.Contains("Se utilizó ruta inversa", log);
        Assert.Contains("tarifa base 8", log);
        Assert.Contains("Monto base: 80", log);
        Assert.Contains("recargo 10%: 8", log);
        Assert.Contains("costo total calculado: 88", log);
    }

    #endregion

    #region Transshipment Tests

    //[Fact]
    void CalcularTarifaEnvio_Transshipment_CalculatesCorrectTotal()
    {
        // Arrange
        decimal kilos = 10m;
        string zonaOrigen = "A";
        string zonaDestino = "C";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("A", "B"), 5m },
            { ("B", "C"), 3m }
            // No direct A->C, will use transshipment A->B->C
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        // A->B: 10 * 5 = 50
        // B->C: 10 * 3 = 30
        // Total: 50 + 30 = 80
        Assert.Equal(80m, resultado);
    }

    //[Fact]
    void CalcularTarifaEnvio_Transshipment_LogMentionsIntermediateCity()
    {
        // Arrange
        decimal kilos = 5m;
        string zonaOrigen = "X";
        string zonaDestino = "Z";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("X", "Y"), 4m },
            { ("Y", "Z"), 6m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        Assert.Contains("Se utilizó transbordo a través de Y", log);
        Assert.Contains("Primer tramo (X→Y): 20", log);
        Assert.Contains("segundo tramo (Y→Z): 30", log);
        Assert.Contains("costo total calculado: 50", log);
    }

    //[Fact]
    void CalcularTarifaEnvio_Transshipment_RoundsToTwoDecimals()
    {
        // Arrange
        decimal kilos = 7.5m;
        string zonaOrigen = "P";
        string zonaDestino = "R";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("P", "Q"), 2.333m },
            { ("Q", "R"), 4.777m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        // P->Q: 7.5 * 2.333 = 17.4975
        // Q->R: 7.5 * 4.777 = 35.8275
        // Total: 17.4975 + 35.8275 = 53.325 → rounded to 53.33
        Assert.Equal(53.33m, resultado);
    }

    #endregion

    #region Accumulated Calculation Tests

    //[Fact]
    void CalcularTarifaEnvio_AccumulatedCalculation_ValidatesSegmentSum()
    {
        // Arrange
        decimal kilos = 12m;
        string zonaOrigen = "START";
        string zonaDestino = "END";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("START", "MID"), 3.5m },
            { ("MID", "END"), 2.25m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        // START->MID: 12 * 3.5 = 42
        // MID->END: 12 * 2.25 = 27
        // Total: 42 + 27 = 69
        Assert.Equal(69m, resultado);
        Assert.Contains("Primer tramo (START→MID): 42", log);
        Assert.Contains("segundo tramo (MID→END): 27", log);
        Assert.Contains("costo total calculado: 69", log);
    }

    //[Fact]
    void CalcularTarifaEnvio_ComplexRouting_PrefersDirectOverTransshipment()
    {
        // Arrange
        decimal kilos = 8m;
        string zonaOrigen = "ALPHA";
        string zonaDestino = "BETA";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("ALPHA", "BETA"), 10m }, // Direct route exists
            { ("ALPHA", "GAMMA"), 4m },
            { ("GAMMA", "BETA"), 3m }  // Transshipment also possible
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas, out var log);

        // Assert
        // Should use direct route: 8 * 10 = 80
        // Not transshipment: (8 * 4) + (8 * 3) = 32 + 24 = 56
        Assert.Equal(80m, resultado);
        Assert.Contains("ALPHA hacia BETA", log);
        Assert.Contains("Costo total calculado: 80", log);
    }

    #endregion
}

}

    





