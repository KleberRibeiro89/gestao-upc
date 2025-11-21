using GestaoUpc.Api.Configurations;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestaoUpc.Api.Configurations;

[ApiController]
[Route("api/[controller]")]
public class GestaoUpcController : ControllerBase
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    public User ContextUser { get; }

    public GestaoUpcController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            string userId = _httpContextAccessor.GetUserIdFromHeader() ?? string.Empty;
            string userAdId = httpContext.GetUserIdAd() ?? string.Empty;

            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
            {
                parsedUserId = Guid.Empty;
            }

            Guid parsedUserAdId;
            if (!Guid.TryParse(userAdId, out parsedUserAdId))
            {
                parsedUserAdId = Guid.Empty;
            }

            ContextUser = new User
            {
                Email = httpContext.GetUserEmail() ?? string.Empty,
                Name = httpContext.GetUserName() ?? string.Empty,
                NavigationId = parsedUserId,
                CreatedBy = parsedUserId
            };
        }
        else
        {
            ContextUser = new User();
        }
    }

    public ObjectResult PrepareResponse<T>(ResponseBase<T> data) => data.Success ? Ok(data.Data) : BadRequest(data);
}
