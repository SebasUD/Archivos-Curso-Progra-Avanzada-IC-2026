using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class BookThemeRepository : Repository<Booktheme>, IBookThemeRepository
{
    public BookThemeRepository(JsonDbContext context) : base(context)
    {
    }
}
