using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using CyberbezpApi.Services.Interfaces;

namespace CyberbezpApi.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public string? GetUserId =>
            User is null ? null : User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
