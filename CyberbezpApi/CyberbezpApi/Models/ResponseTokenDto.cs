namespace CyberbezpApi.Models
{
    public class ResponseTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool hasPasswordExpired { get; set; } = false;
    }
}
