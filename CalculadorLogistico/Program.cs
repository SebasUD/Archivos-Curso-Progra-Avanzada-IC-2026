using CalculadorLogistico;

var calculador = new CalculadorLogistico.CalculadorLogistico();

// Example usage with direct route
var tarifas = new Dictionary<(string, string), decimal>
{
    { ("A", "B"), 5m },
    { ("B", "C"), 3m },
    { ("A", "C"), 7m }
};

var costo = calculador.CalcularTarifaEnvio(10m, "A", "B", tarifas, out var log);
Console.WriteLine($"Costo: {costo}");
Console.WriteLine($"Log: {log}");
