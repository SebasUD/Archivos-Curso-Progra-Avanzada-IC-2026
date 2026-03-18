using MyLibrary.DbModel.Entities;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

namespace MyLibrary.JsonRepositoryModel.Repositories;

public class BorrowRepository : Repository<Borrow>, IBorrowRepository
{
    public BorrowRepository(JsonDbContext context) : base(context)
    {
    }
}
