using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;
using GestaoUpc.Domain.Entities;
using GestaoUpc.Domain.Repositories;
using GestaoUpc.Domain.Services;

namespace GestaoUpc.Domain.AppService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _createUserValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IValidator<CreateUserRequest> createUserValidator,
        IValidator<UpdateUserRequest> updateUserValidator,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _mapper = mapper;
    }

    public async Task<(UserResponse? User, ValidationResult? ValidationResult)> CreateAsync(CreateUserRequest request)
    {
        var validationResult = await _createUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        var existingUser = await GetUserByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException($"Já existe um usuário com o email {request.Email}");

        var user = _mapper.Map<User>(request);
        var createdUser = await _userRepository.AddAsync(user);
        return (_mapper.Map<UserResponse>(createdUser), null);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? _mapper.Map<UserResponse>(user) : null;
    }

    public async Task<UserResponse?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var user = await GetUserByEmailAsync(email);
        return user != null ? _mapper.Map<UserResponse>(user) : null;
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<List<UserResponse>>(users);
    }

    public async Task<(UserResponse? User, ValidationResult? ValidationResult)> UpdateAsync(UpdateUserRequest request)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        var existingUser = await _userRepository.GetByIdAsync(request.NavigationId);
        if (existingUser == null)
            throw new InvalidOperationException($"Usuário com ID {request.NavigationId} não encontrado");

        if (existingUser.Email != request.Email)
        {
            var userWithEmail = await GetUserByEmailAsync(request.Email);
            if (userWithEmail != null && userWithEmail.NavigationId != request.NavigationId)
                throw new InvalidOperationException($"Já existe um usuário com o email {request.Email}");
        }

        var user = _mapper.Map<User>(request);
        var updatedUser = await _userRepository.UpdateAsync(user);
        return (_mapper.Map<UserResponse>(updatedUser), null);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new InvalidOperationException($"Usuário com ID {id} não encontrado");

        await _userRepository.DeleteAsync(user);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null && user.Active;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var user = await GetUserByEmailAsync(email);
        return user != null;
    }

    private async Task<User?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.GetAllAsync(u => u.Email == email && u.Active);
        return users.FirstOrDefault();
    }
}

