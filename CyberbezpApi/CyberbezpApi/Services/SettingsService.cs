namespace CyberbezpApi.Services
{
    public class SettingsService
    {
        public bool isEnabledPasswordRequriments = true;
        public int PasswordMinLength { get; set; } = 14;
        public int PasswordExpirationTime { get; set; } = 20;
        public int UserSession { get; set; } = 10;
        public int MaximumNumberOfAttempts { get; set; } = 5;
    }
}
