using GestaoUpc.Domain.Data.Contexts;
using GestaoUpc.Domain.Entities;
using GestaoUpc.Domain.Repositories;

namespace GestaoUpc.Domain.Data.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(GestaoUpcDbContext db) : base(db)
    {
    }
}
