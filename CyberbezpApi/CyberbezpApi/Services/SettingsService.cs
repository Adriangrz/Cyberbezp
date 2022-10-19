namespace CyberbezpApi.Services
{
    public class SettingsService
    {
        public bool isEnabledPasswordRequriments = true;
        public int PasswordMinLength { get; set; } = 14;
        public int PasswordExpirationTime { get; set; } = 20;
    }
}
