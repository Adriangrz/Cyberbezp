using AutoMapper;
using CyberbezpApi.Database.Entities;
using CyberbezpApi.Database;
using CyberbezpApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CyberbezpApi.Exceptions;
using CyberbezpApi.Models;
using Microsoft.EntityFrameworkCore;
using CyberbezpApi.Authorization;

namespace CyberbezpApi.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IAuthService _authService;

        public UserService(UserManager<User> userManager, IConfiguration configuration, IMapper mapper, ApplicationDbContext dbContext, IAuthorizationService authorizationService, IUserContextService userContextService, IAuthService authService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _authService = authService;
        }

        public async Task<UserDto> ChangeName(string userId, string name)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new NotFoundException("Użytkownik nie istnieje");
            user.UserName = name;
            await _userManager.UpdateAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task ChangePassword(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("Użytkownik nie istnieje");

            _authService.CheckPassword(password);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
                throw new Exception("Nie udało się zmienić hasła");

        }
        public async Task BlockUser(string userId, bool enabled)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("Użytkownik nie istnieje");

            await _userManager.SetLockoutEnabledAsync(user, enabled);
        }
        public async Task<List<UserDto>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersDtos = _mapper.Map<List<UserDto>>(users);
            return usersDtos;
        }
        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("Użytkownik nie istnieje");

            await _userManager.DeleteAsync(user);
        }
    }
}
