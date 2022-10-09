using CyberbezpApi.Database.Entities;
using CyberbezpApi.Models;

namespace CyberbezpApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task ChangePassword(ChangePasswordDto changePasswordDto);
        Task<ResponseTokenDto> AuthenticationAsync(LoginDto loginDto);
        ResponseTokenDto GenerateJWT(User userInfo, List<string> userRoles);
        Task RegistrationAsync(RegistrationDto registrationDto);
        void EnableOrDisablePasswordRequirements(bool isEnable);
        void ChangePasswordMinLength(int minLength);
        void SetPasswordExpirationTime(int time);
        void CheckPassword(string password);
    }
}
