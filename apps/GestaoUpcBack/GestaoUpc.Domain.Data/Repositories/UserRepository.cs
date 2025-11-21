using GestaoUpc.Domain.Data.Contexts;
using GestaoUpc.Domain.Data.Extensions;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;
using GestaoUpc.Domain.Entities;
using GestaoUpc.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GestaoUpc.Domain.Data.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(GestaoUpcDbContext db) : base(db)
    {
    }

    public async Task<DynamicQueryResult<UserResponse>> GetPagedAsync(UserPagedRequest request)
    {
        var query = _dbSet.AsNoTracking()
                  .Where(x => x.Active);

        return await query
            .Select(x => new UserResponse
            {
                NavigationId = x.NavigationId,
                Name = x.Name,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToPagedAsync(request); ;
    }
}
