using CyberbezpApi.Database.Entities;
using CyberbezpApi.Models;

namespace CyberbezpApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task ChangePassword(ChangePasswordDto changePasswordDto);
        Task<ResponseTokenDto> AuthenticationAsync(LoginDto loginDto);
        ResponseTokenDto GenerateJWT(User userInfo, string userRole);
        Task RegistrationAsync(RegistrationDto registrationDto);
        void EnableOrDisablePasswordRequirements(bool isEnable);
        void ChangePasswordMinLength(int minLength);
        void SetPasswordExpirationTime(int time);
        void CheckPassword(string password);
        void SetMaximumNumberOfAttempts(int time);
        void SetUserSession(int time);
        SettingsDto GetAllSettings();
    }
}
