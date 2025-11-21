using AutoMapper;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;

namespace GestaoUpc.Domain.Mappings;

public class RequestToDomainMapping : Profile
{
    public RequestToDomainMapping()
    {
        CreateMap<CreateUserRequest, Entities.User>()
            .ForMember(dest => dest.Active, opt => opt.MapFrom(_ => true));

        CreateMap<UpdateUserRequest, Entities.User>()
            .ForMember(dest => dest.Active, opt => opt.MapFrom(_ => true));

    }
}
