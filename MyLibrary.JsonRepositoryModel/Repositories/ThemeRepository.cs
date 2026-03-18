using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class ThemeRepository : Repository<Theme>, IThemeRepository
{
    public ThemeRepository(JsonDbContext context) : base(context)
    {
    }
}
