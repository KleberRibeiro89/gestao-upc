using Microsoft.AspNetCore.Mvc;
using GestaoUpc.Domain.Services;
using GestaoUpc.Domain.DTOs.Requests.UserRequestSource;
using GestaoUpc.Domain.DTOs.Responses.UserResponseSource;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Api.Configurations;

namespace GestaoUpc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : GestaoUpcController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    public UserController(
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        ILogger<UserController> logger) : base(httpContextAccessor)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var (user, validationResult) = await _userService.CreateAsync(request);
            
            if (validationResult != null && !validationResult.IsValid)
            {
                return BadRequest(ResponseBase.RequestError("Erro de validação", validationResult));
            }

            if (user == null)
                return StatusCode(500, ResponseBase.Fail("Erro ao criar usuário"));

            return CreatedAtAction(nameof(GetById), new { id = user.NavigationId }, ResponseBase.Ok(user));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao criar usuário");
            return Conflict(ResponseBase.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFound(ResponseBase.Fail($"Usuário com ID {id} não encontrado"));

            return Ok(ResponseBase.Ok(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {Id}", id);
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém usuários paginados
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] UserPagedRequest request)
    {
        try
        {
            var result = await _userService.GetPagedAsync(request);
            return PrepareResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários paginados");
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Obtém um usuário por email
    /// </summary>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(email);
            
            if (user == null)
                return NotFound(ResponseBase.Fail($"Usuário com email {email} não encontrado"));

            return Ok(ResponseBase.Ok(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Atualiza um usuário
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (id != request.NavigationId)
            return BadRequest(ResponseBase.Fail("ID da URL não corresponde ao ID do corpo da requisição"));

        try
        {
            var (user, validationResult) = await _userService.UpdateAsync(request);
            
            if (validationResult != null && !validationResult.IsValid)
            {
                return BadRequest(ResponseBase.RequestError("Erro de validação", validationResult));
            }

            if (user == null)
                return StatusCode(500, ResponseBase.Fail("Erro ao atualizar usuário"));

            return Ok(ResponseBase.Ok(user));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao atualizar usuário");
            
            if (ex.Message.Contains("não encontrado"))
                return NotFound(ResponseBase.Fail(ex.Message));
            
            return Conflict(ResponseBase.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário");
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Deleta um usuário
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao deletar usuário");
            return NotFound(ResponseBase.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar usuário");
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Verifica se um usuário existe
    /// </summary>
    [HttpGet("{id}/exists")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> Exists(Guid id)
    {
        try
        {
            var exists = await _userService.ExistsAsync(id);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do usuário");
            return StatusCode(500, ResponseBase.Fail("Erro interno do servidor"));
        }
    }
}

