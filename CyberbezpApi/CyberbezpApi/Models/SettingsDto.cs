namespace CyberbezpApi.Models
{
    public class SettingsDto
    {
        public bool isEnabledPasswordRequirements { get; set; }
        public int PasswordMinLength { get; set; }
        public int PasswordExpirationTime { get; set; }
    }
}
