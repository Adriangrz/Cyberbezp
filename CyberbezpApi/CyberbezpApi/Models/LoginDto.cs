namespace CyberbezpApi.Models
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? OneTimePassword { get; set; }
    }
}
