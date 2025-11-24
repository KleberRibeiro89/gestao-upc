using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using GestaoUpc.Domain.DTOs.Requests.PagedRequest;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;
using GestaoUpc.Domain.Entities;
using GestaoUpc.Domain.Repositories;
using GestaoUpc.Domain.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GestaoUpc.Domain.AppService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _createUserValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;
    private readonly IValidator<ChangePasswordOnFirstAccessRequest> _changePasswordOnFirstAccessValidator;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public UserService(
        IUserRepository userRepository,
        IValidator<CreateUserRequest> createUserValidator,
        IValidator<UpdateUserRequest> updateUserValidator,
        IValidator<ChangePasswordOnFirstAccessRequest> changePasswordOnFirstAccessValidator,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _changePasswordOnFirstAccessValidator = changePasswordOnFirstAccessValidator;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
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

        var defaultPassword = _configuration["UserSettings:DefaultPassword"] ?? "123456";
        user.Password = _passwordHasher.HashPassword(defaultPassword);
        user.IsFirstAccess = true;

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

    public async Task<ResponseBase<DynamicQueryResult<UserResponse>>> GetPagedAsync(UserPagedRequest request)
    {
        if (!string.IsNullOrEmpty(request.FilterString))
            request.Filter = JsonConvert.DeserializeObject<List<PropertyFilter>>(request.FilterString);

        if (!string.IsNullOrEmpty(request.OrderByString))
            request.OrderBy = JsonConvert.DeserializeObject<List<PropertySort>>(request.OrderByString);

        var result = await _userRepository.GetPagedAsync(request);

        // Mapeia os resultados de User para UserResponse
        var mappedResult = new DynamicQueryResult<UserResponse>(result.PageSize)
        {
            PageNumber = result.PageNumber,
            TotalRows = result.TotalRows,
            ResultType = result.ResultType,
            Result = _mapper.Map<List<UserResponse>>(result.Result.ToList())
        };

        return ResponseBase.Ok(mappedResult);
    }

    public async Task<(UserResponse? User, ValidationResult? ValidationResult)> UpdateAsync(UpdateUserRequest request)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        var existingUser = await _userRepository.GetByIdAsync(request.NavigationId) ?? throw new InvalidOperationException($"Usuário com ID {request.NavigationId} não encontrado");
        if (existingUser.Email != request.Email)
        {
            var userWithEmail = await GetUserByEmailAsync(request.Email);
            if (userWithEmail != null && userWithEmail.NavigationId != request.NavigationId)
                throw new InvalidOperationException($"Já existe um usuário com o email {request.Email}");
        }

        var user = _mapper.Map<User>(request);

        // Criptografa a senha apenas se uma nova senha foi fornecida
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = _passwordHasher.HashPassword(request.Password);
        }
        else
        {
            // Mantém a senha existente se não foi fornecida uma nova
            user.Password = existingUser.Password;
        }

        var updatedUser = await _userRepository.UpdateAsync(user);
        return (_mapper.Map<UserResponse>(updatedUser), null);
    }

    public async Task<(UserResponse? User, ValidationResult? ValidationResult)> ChangePasswordOnFirstAccessAsync(ChangePasswordOnFirstAccessRequest request)
    {
        var validationResult = await _changePasswordOnFirstAccessValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return (null, validationResult);
        }

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new InvalidOperationException($"Usuário com ID {request.UserId} não encontrado");

        if (!user.IsFirstAccess)
            throw new InvalidOperationException("Este usuário já realizou o primeiro acesso. Use o endpoint de alteração de senha padrão.");

        // Atualiza a senha e marca que não é mais o primeiro acesso
        user.Password = _passwordHasher.HashPassword(request.NewPassword);
        user.IsFirstAccess = false;

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

