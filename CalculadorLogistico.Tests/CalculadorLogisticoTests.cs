using System.Collections.Generic;
using Xunit;
using CalculadorLogistico;

namespace CalculadorLogistico.Tests;

public class CalculadorLogisticoTests
{
    private readonly global::CalculadorLogistico.CalculadorLogistico _calculador = new();

    #region Direct Route Tests

    [Fact]
    public void CalcularTarifaEnvio_WithDirectRoute_ReturnsCorrectTotal()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(50m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_WithSmallWeight_ReturnsCorrectTotal()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(15.5m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_WithLargeWeight_ReturnsCorrectTotal()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(2750m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_WithDecimalWeight_ReturnsCorrectTotal()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(67.375m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_WithZeroWeight_ReturnsZero()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(0m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_WithMultipleRoutes_SelectsCorrectRoute()
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
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(160m, resultado);
    }

    [Fact]
    public void CalcularTarifaEnvio_RouteNotFound_ReturnsZero()
    {
        // Arrange
        decimal kilos = 15m;
        string zonaOrigen = "A";
        string zonaDestino = "B";
        var tarifas = new Dictionary<(string, string), decimal>
        {
            { ("X", "Y"), 5m }
        };

        // Act
        decimal resultado = _calculador.CalcularTarifaEnvio(kilos, zonaOrigen, zonaDestino, tarifas);

        // Assert
        Assert.Equal(0m, resultado);
    }

    #endregion
}
