using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class BookAuthorRepository : Repository<Bookauthor>, IBookAuthorRepository
{
    public BookAuthorRepository(JsonDbContext context) : base(context)
    {
    }
}
