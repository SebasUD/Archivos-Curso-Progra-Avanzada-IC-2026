using System.Text.Json;
using MyLibrary.DbModel.Entities;

namespace MyLibrary.JsonRepositoryModel.Context;

public class JsonDbContext
{
    private readonly string _dataFolderPath;
    
    public List<Book> Books { get; set; } = new();
    public List<Author> Authors { get; set; } = new();
    public List<User> Users { get; set; } = new();
    public List<Theme> Themes { get; set; } = new();
    public List<Bookauthor> Bookauthors { get; set; } = new();
    public List<Bookcopy> Bookcopies { get; set; } = new();
    public List<Booktheme> Bookthemes { get; set; } = new();
    public List<Borrow> Borrows { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonDbContext(string? dataFolderPath = null)
    {
        _dataFolderPath = dataFolderPath ?? Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "test-data");
        LoadData();
    }

    private void LoadData()
    {
        Books = LoadFromJson<Book>("books.json");
        Authors = LoadFromJson<Author>("authors.json");
        Users = LoadFromJson<User>("users.json");
        Themes = LoadFromJson<Theme>("themes.json");
        Bookauthors = LoadFromJson<Bookauthor>("bookauthors.json");
        Bookcopies = LoadFromJson<Bookcopy>("bookcopies.json");
        Bookthemes = LoadFromJson<Booktheme>("bookthemes.json");
        Borrows = LoadFromJson<Borrow>("borrows.json");
    }

    private List<T> LoadFromJson<T>(string fileName)
    {
        var filePath = Path.Combine(_dataFolderPath, fileName);
        if (!File.Exists(filePath))
        {
            return new List<T>();
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
    }

    public void SaveChanges()
    {
        SaveToJson("books.json", Books);
        SaveToJson("authors.json", Authors);
        SaveToJson("users.json", Users);
        SaveToJson("themes.json", Themes);
        SaveToJson("bookauthors.json", Bookauthors);
        SaveToJson("bookcopies.json", Bookcopies);
        SaveToJson("bookthemes.json", Bookthemes);
        SaveToJson("borrows.json", Borrows);
    }

    private void SaveToJson<T>(string fileName, List<T> data)
    {
        var filePath = Path.Combine(_dataFolderPath, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
