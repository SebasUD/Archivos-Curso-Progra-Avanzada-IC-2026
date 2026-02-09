using System.Linq;

namespace MyLibrary.DbModel.Entities;

public partial class Book
{
    public string AllBookAuthorsFirstNameLastName =>
        string.Join(" - ", Bookauthors
            .Where(ba => ba.Author != null)
            .Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}"));

    public string AllBookAuthorsLastNameFirstName =>
        string.Join(" - ", Bookauthors
            .Where(ba => ba.Author != null)
            .Select(ba => $"{ba.Author.LastName} {ba.Author.FirstName}"));

    public string AllBookThemeNames =>
        string.Join(" - ", Bookthemes
            .Where(bt => bt.Theme != null)
            .Select(bt => bt.Theme.Name));
}
