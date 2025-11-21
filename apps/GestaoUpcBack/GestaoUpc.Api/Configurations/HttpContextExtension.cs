using System.Security.Claims;

namespace GestaoUpc.Api.Configurations;

public static class HttpContextExtension
{
    private const string UserIdHeader = "User-Id"; // TODO: definir com base no que vir do token
    private const string UserIdAdKey = "Id-Ad"; // TODO: definir com base no que vir do token

    private static ClaimsPrincipal? GetUser(HttpContext? context) => context?.User;
    public static string? GetUserIdFromHeader(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;

        // Primeiro tenta extrair do JWT token (claims)
        var userIdFromClaims = GetUser(httpContext)?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userIdFromClaims))
        {
            return userIdFromClaims;
        }

        // Fallback para header (se necessário)
        if (httpContext?.Request.Headers.ContainsKey(UserIdHeader) == true)
        {
            return httpContext.Request.Headers[UserIdHeader].FirstOrDefault();
        }

        return null;
    }

    public static string? GetUserIdAd(this HttpContext context) => GetUser(context)?.FindFirstValue(UserIdAdKey);

    public static string? GetUserEmail(this HttpContext context) => GetUser(context)?.FindFirstValue(ClaimTypes.Email);

    public static string? GetUserName(this HttpContext? context)
    {
        var user = GetUser(context);
        if (user == null) return null;

        return user.FindFirstValue(ClaimTypes.Name) ??
               user.FindFirstValue("name") ??
               $"{user.FindFirstValue(ClaimTypes.GivenName)} {user.FindFirstValue(ClaimTypes.Surname)}";
    }
}
