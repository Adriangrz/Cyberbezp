using Microsoft.AspNetCore.Identity;

namespace CyberbezpApi.Database.Entities
{
    public class User: IdentityUser
    {
        public bool IsFirstLogin { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime UserLockOutDate { get; set; }
        public string? OneTimePassword { get; set; }
        public bool IsFileBlock { get; set; }
        public DateTime FirstAccess { get; set; }
    }
}
