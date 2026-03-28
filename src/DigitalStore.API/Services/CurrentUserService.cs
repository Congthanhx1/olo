using DigitalStore.Application.Common.Interfaces;
using System.Security.Claims;

namespace DigitalStore.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext)
        => _httpContext = httpContext;

    public int UserId
    {
        get
        {
            var claim = _httpContext.HttpContext?.User
                            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id)
                   ? id
                   : throw new UnauthorizedAccessException("Token không hợp lệ.");
        }
    }

    public string Role
    {
        get
        {
            return _httpContext.HttpContext?.User
                       .FindFirst(ClaimTypes.Role)?.Value ?? "Anonymous";
        }
    }
}
