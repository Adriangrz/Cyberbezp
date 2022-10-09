using System.Security.Claims;

namespace CyberbezpApi.Services.Interfaces
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        string? GetUserId { get; }
    }
}
