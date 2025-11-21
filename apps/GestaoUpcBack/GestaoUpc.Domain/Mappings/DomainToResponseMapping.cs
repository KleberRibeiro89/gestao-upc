using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;
using GestaoUpc.Domain.Entities;

namespace GestaoUpc.Domain.Mappings;

public class DomainToResponseMapping : AutoMapper.Profile
{
    public DomainToResponseMapping()
    {
        // Entity to Response
        CreateMap<User, UserResponse>();
    }
}
