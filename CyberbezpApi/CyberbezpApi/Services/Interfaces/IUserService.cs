using CyberbezpApi.Models;

namespace CyberbezpApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> ChangeName(string userId, string name);
        Task ChangePassword(string userId, string password);
        Task<List<UserDto>> GetUsers();
        Task DeleteUser(string userId);
        Task BlockUser(string userId);
    }
}
