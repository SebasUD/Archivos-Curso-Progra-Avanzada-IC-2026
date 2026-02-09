using System;
using Xunit;
using MyLetterComparer;

namespace MyLetterComparer.Tests;

public class LetterComparerTests
{
    private readonly LetterComparer _comparer = new();

    #region CompareLetter Tests - Basic Functionality

    [Fact]
    public void CompareLetter_SameLetters_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('a', 'a');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_SameLettersDifferentCase_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('A', 'a');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_FirstLetterBeforeSecond_ReturnsNegativeOne()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('a', 'b');

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CompareLetter_FirstLetterAfterSecond_ReturnsOne()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('z', 'a');

        // Assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData('a', 'b', -1)]
    [InlineData('c', 'd', -1)]
    [InlineData('x', 'y', -1)]
    public void CompareLetter_FirstLetterBeforeVariousCombinations_ReturnsNegativeOne(char letter1, char letter2, int expected)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(letter1, letter2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('b', 'a', 1)]
    [InlineData('d', 'c', 1)]
    [InlineData('z', 'x', 1)]
    public void CompareLetter_FirstLetterAfterVariousCombinations_ReturnsOne(char letter1, char letter2, int expected)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(letter1, letter2);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region CompareLetter Tests - Digit Comparisons

    [Fact]
    public void CompareLetter_SameDigits_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('5', '5');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_FirstDigitBeforeSecond_ReturnsNegativeOne()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('1', '5');

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CompareLetter_FirstDigitAfterSecond_ReturnsOne()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('9', '2');

        // Assert
        Assert.Equal(1, result);
    }

    #endregion

    #region ArgumentValidation Tests - Exception Scenarios

    [Fact]
    public void CompareLetter_LetterAndDigit_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('a', '5'));
    }

    [Fact]
    public void CompareLetter_DigitAndLetter_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('3', 'z'));
    }

    [Theory]
    [InlineData('a', '0')]
    [InlineData('z', '9')]
    [InlineData('A', '1')]
    public void CompareLetter_MixedLetterAndDigit_ThrowsArgumentException(char letter, char digit)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter(letter, digit));
    }

    [Fact]
    public void CompareLetter_InvalidCharacterFirstParameter_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('!', 'a'));
    }

    [Fact]
    public void CompareLetter_InvalidCharacterSecondParameter_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('a', '@'));
    }

    [Theory]
    [InlineData('!', 'a')]
    [InlineData('#', 'z')]
    [InlineData('$', 'A')]
    [InlineData('%', '5')]
    public void CompareLetter_SpecialCharactersAsFirstParameter_ThrowsArgumentException(char specialChar, char validChar)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter(specialChar, validChar));
    }

    [Theory]
    [InlineData('a', '!')]
    [InlineData('z', '#')]
    [InlineData('A', '$')]
    [InlineData('5', '%')]
    public void CompareLetter_SpecialCharactersAsSecondParameter_ThrowsArgumentException(char validChar, char specialChar)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter(validChar, specialChar));
    }

    [Fact]
    public void CompareLetter_BothInvalidCharacters_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('!', '@'));
    }

    [Fact]
    public void CompareLetter_ExceptionMessageContainsFirstParameter()
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('a', '5'));
        Assert.Contains("'a'", ex.Message);
    }

    [Fact]
    public void CompareLetter_ExceptionMessageContainsSecondParameter()
    {
        // Arrange, Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _comparer.CompareLetter('a', '5'));
        Assert.Contains("'5'", ex.Message);
    }

    #endregion

    #region CompareLetter Tests - Accented Characters (Latin American)

    [Fact]
    public void CompareLetter_AccentedAAndRegularA_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('á', 'a');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_RegularAAndAccentedA_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('a', 'á');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_TwoAccentedAs_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('á', 'á');

        // Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData('á', 'a')]
    [InlineData('é', 'e')]
    [InlineData('í', 'i')]
    [InlineData('ó', 'o')]
    [InlineData('ú', 'u')]
    public void CompareLetter_AccentedVowelAndPlainVowel_ReturnsZero(char accentedChar, char plainChar)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(accentedChar, plainChar);

        // Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData('a', 'á')]
    [InlineData('e', 'é')]
    [InlineData('i', 'í')]
    [InlineData('o', 'ó')]
    [InlineData('u', 'ú')]
    public void CompareLetter_PlainVowelAndAccentedVowel_ReturnsZero(char plainChar, char accentedChar)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(plainChar, accentedChar);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_UmlautUAndPlainU_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('ü', 'u');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_LowercaseNAndLowercaseNTilde_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('n', 'ñ');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_NTildeAndN_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('ñ', 'n');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_CapitalNTildeAndLowercaseN_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('Ñ', 'n');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_LowercaseNAndCapitalNTilde_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('n', 'Ñ');

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_CapitalNTildeAndCapitalN_ReturnsZero()
    {
        // Arrange & Act
        int result = _comparer.CompareLetter('Ñ', 'N');

        // Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData('á', 'b', -1)]
    [InlineData('é', 'f', -1)]
    [InlineData('ñ', 'o', -1)]
    public void CompareLetter_AccentedCharBeforeOtherChar_ReturnsNegativeOne(char accentedChar, char otherChar, int expected)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(accentedChar, otherChar);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('z', 'á', 1)]
    [InlineData('x', 'é', 1)]
    [InlineData('w', 'ñ', 1)]
    public void CompareLetter_CharAfterAccentedChar_ReturnsOne(char otherChar, char accentedChar, int expected)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(otherChar, accentedChar);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('Á', 'a')]
    [InlineData('É', 'e')]
    [InlineData('Í', 'i')]
    [InlineData('Ó', 'o')]
    [InlineData('Ú', 'u')]
    public void CompareLetter_CapitalAccentedVowelAndLowercasePlainVowel_ReturnsZero(char capitalAccented, char lowercase)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(capitalAccented, lowercase);

        // Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData('a', 'Á')]
    [InlineData('e', 'É')]
    [InlineData('i', 'Í')]
    [InlineData('o', 'Ó')]
    [InlineData('u', 'Ú')]
    public void CompareLetter_LowercasePlainVowelAndCapitalAccentedVowel_ReturnsZero(char lowercase, char capitalAccented)
    {
        // Arrange & Act
        int result = _comparer.CompareLetter(lowercase, capitalAccented);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareLetter_MultipleAccentsOnSameBase_ReturnsZero()
    {
        // Arrange & Act
        int result1 = _comparer.CompareLetter('á', 'à');
        int result2 = _comparer.CompareLetter('à', 'ä');

        // Assert
        Assert.Equal(0, result1);
        Assert.Equal(0, result2);
    }

    #endregion

    #region ArgumentValidation Tests - Direct Method

    [Fact]
    public void ArgumentValidation_TwoLetters_NoExceptionThrown()
    {
        // Arrange & Act
        var ex = Record.Exception(() => _comparer.ArgumentValidation('a', 'b'));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void ArgumentValidation_TwoDigits_NoExceptionThrown()
    {
        // Arrange & Act
        var ex = Record.Exception(() => _comparer.ArgumentValidation('1', '5'));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void ArgumentValidation_LetterAndDigit_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.ArgumentValidation('a', '5'));
    }

    [Fact]
    public void ArgumentValidation_InvalidCharacterInFirstPosition_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.ArgumentValidation('!', 'a'));
    }

    [Fact]
    public void ArgumentValidation_InvalidCharacterInSecondPosition_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => _comparer.ArgumentValidation('a', '!'));
    }

    #endregion
}
