using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class BookCopyRepository : Repository<Bookcopy>, IBookCopyRepository
{
    public BookCopyRepository(JsonDbContext context) : base(context)
    {
    }
}
