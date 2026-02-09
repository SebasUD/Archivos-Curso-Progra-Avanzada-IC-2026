namespace MyLetterComparer;

using System.Text;

public class LetterComparer
{
    /// <summary>
    /// Removes accents from a character, treating accented vowels and ñ as their base equivalents.
    /// For example: á, é, í, ó, ú, ü → a, e, i, o, u, u and ñ → n
    /// </summary>
    /// <param name="character">The character to remove accents from</param>
    /// <returns>The character without accents</returns>
    private char RemoveAccents(char character)
    {
        // Handle specific Latin American accented characters
        return character switch
        {
            'á' or 'à' or 'ä' or 'â' => 'a',
            'Á' or 'À' or 'Ä' or 'Â' => 'a',
            'é' or 'è' or 'ë' or 'ê' => 'e',
            'É' or 'È' or 'Ë' or 'Ê' => 'e',
            'í' or 'ì' or 'ï' or 'î' => 'i',
            'Í' or 'Ì' or 'Ï' or 'Î' => 'i',
            'ó' or 'ò' or 'ö' or 'ô' => 'o',
            'Ó' or 'Ò' or 'Ö' or 'Ô' => 'o',
            'ú' or 'ù' or 'ü' or 'û' => 'u',
            'Ú' or 'Ù' or 'Ü' or 'Û' => 'u',
            'ñ' => 'n',
            'Ñ' => 'n',
            _ => character
        };
    }

    /// <summary>
    /// Validates that both characters are either letters or digits,
    /// and that they are the same type (both letters or both digits).
    /// </summary>
    /// <param name="char1">The first character</param>
    /// <param name="char2">The second character</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public void ArgumentValidation(char char1, char char2)
    {
        bool isChar1Letter = char.IsLetter(char1);
        bool isChar2Letter = char.IsLetter(char2);
        bool isChar1Digit = char.IsDigit(char1);
        bool isChar2Digit = char.IsDigit(char2);

        if ((isChar1Letter && isChar2Digit) || (isChar1Digit && isChar2Letter))
        {
            throw new ArgumentException(
                $"Error: Cannot compare a letter with a number! '{char1}' is a {(isChar1Letter ? "letter" : "number")}, " +
                $"'{char2}' is a {(isChar2Letter ? "letter" : "number")}");
        }

        if (!isChar1Letter && !isChar1Digit)
        {
            throw new ArgumentException($"Error: '{char1}' is neither a letter nor a digit!");
        }

        if (!isChar2Letter && !isChar2Digit)
        {
            throw new ArgumentException($"Error: '{char2}' is neither a letter nor a digit!");
        }
    }

    /// <summary>
    /// Compares two letters alphabetically, treating accented characters as their base equivalents.
    /// For example: á and a are considered the same, ñ and n are considered the same.
    /// </summary>
    /// <param name="letter1">The first letter</param>
    /// <param name="letter2">The second letter</param>
    /// <returns>
    /// 0 if the letters are the same
    /// -1 if the first letter is alphabetically before the second
    /// 1 if the first letter is alphabetically after the second
    /// </returns>
    public int CompareLetter(char letter1, char letter2)
    {
        ArgumentValidation(letter1, letter2);

        // Convert to lowercase and remove accents for comparison
        char l1 = char.ToLower(RemoveAccents(letter1));
        char l2 = char.ToLower(RemoveAccents(letter2));

        if (l1 == l2)
            return 0;
        else if (l1 < l2)
            return -1;
        else
            return 1;
    }
}
