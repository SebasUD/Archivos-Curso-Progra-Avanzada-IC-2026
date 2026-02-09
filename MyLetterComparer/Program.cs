using MyLetterComparer;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run <letter1> <letter2>");
    Console.WriteLine("Example: dotnet run a b");
    return;
}

char char1 = args[0][0];
char char2 = args[1][0];

var comparer = new LetterComparer();

try
{
    int result = comparer.CompareLetter(char1, char2);

    bool isChar1Letter = char.IsLetter(char1);
    string typeLabel = isChar1Letter ? "Letters" : "Numbers";
    Console.WriteLine($"=== {typeLabel} Comparer ===");
    Console.WriteLine();
    Console.WriteLine($"Comparing '{char1}' and '{char2}':");
    Console.WriteLine();

    if (result == 0)
        Console.WriteLine($"Result: {result} - The values are the same");
    else if (result == -1)
        Console.WriteLine($"Result: {result} - '{char1}' comes before '{char2}'");
    else
        Console.WriteLine($"Result: {result} - '{char1}' comes after '{char2}'");
}
catch (ArgumentException ex)
{
    Console.WriteLine(ex.Message);
}
