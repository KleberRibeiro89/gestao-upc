using FluentValidation.Results;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;

namespace GestaoUpc.Domain.Services;

public interface IUserService
{
    Task<(UserResponse? User, ValidationResult? ValidationResult)> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> GetByIdAsync(Guid id);
    Task<UserResponse?> GetByEmailAsync(string email);
    Task<List<UserResponse>> GetAllAsync();
    Task<ResponseBase<DynamicQueryResult<UserResponse>>> GetPagedAsync(UserPagedRequest request);
    Task<(UserResponse? User, ValidationResult? ValidationResult)> UpdateAsync(UpdateUserRequest request);
    Task<(UserResponse? User, ValidationResult? ValidationResult)> ChangePasswordOnFirstAccessAsync(ChangePasswordOnFirstAccessRequest request);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}

