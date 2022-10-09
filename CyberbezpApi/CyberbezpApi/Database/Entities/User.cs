using Microsoft.AspNetCore.Identity;

namespace CyberbezpApi.Database.Entities
{
    public class User: IdentityUser
    {
        public bool IsFirstLogin { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
    }
}
