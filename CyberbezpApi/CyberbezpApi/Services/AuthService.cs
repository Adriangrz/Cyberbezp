using AutoMapper;
using CyberbezpApi.Database.Entities;
using CyberbezpApi.Database;
using CyberbezpApi.Models;
using CyberbezpApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CyberbezpApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CyberbezpApi.Authorization;

namespace CyberbezpApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly SettingsService _settingsService;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, IMapper mapper, ApplicationDbContext dbContext, IAuthorizationService authorizationService, IUserContextService userContextService, SettingsService settingsService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _settingsService = settingsService;
        }
        public async Task RegistrationAsync(RegistrationDto registrationDto)
        {
            var userData = _mapper.Map<User>(registrationDto);
            userData.LastPasswordChangedDate = DateTime.Now;
            userData.IsFirstLogin = true;

            if (await _userManager.FindByEmailAsync(userData.Email) is not null)
                throw new BadRequestException("Użytkownik już istnieje");

            if (!(registrationDto.Password.Length >= _settingsService.PasswordMinLength))
                throw new UnauthorizedException($"Hasło musi mieć długość co najmniej {_settingsService.PasswordMinLength} znaków");

            if (!registrationDto.Password.Any(char.IsDigit) && _settingsService.isEnabledPasswordRequriments)
                throw new UnauthorizedException("Hasło musi mieć co najmniej jedną cyfrę");

            var result = await _userManager.CreateAsync(userData, registrationDto.Password);
            if (!result.Succeeded)
                throw new Exception();

            result = await _userManager.AddToRoleAsync(userData, "User");
            if (!result.Succeeded)
                throw new Exception();

        }
        public void EnableOrDisablePasswordRequirements(bool isEnable)
        {
            _settingsService.isEnabledPasswordRequriments = isEnable;
        }

        public void ChangePasswordMinLength(int minLength)
        {
            _settingsService.PasswordMinLength = minLength;
        }

        public void SetPasswordExpirationTime(int time)
        {
            _settingsService.PasswordExpirationTime = time;
        }

        public SettingsDto GetAllSettings()
        {
            var settings = new SettingsDto()
            {
                PasswordExpirationTime = _settingsService.PasswordExpirationTime,
                PasswordMinLength = _settingsService.PasswordMinLength,
                isEnabledPasswordRequirements= _settingsService.isEnabledPasswordRequriments,
            };
            return settings;
        }

        public void CheckPassword(string password)
        {
            if (!(password.Length >= _settingsService.PasswordMinLength))
                throw new UnauthorizedException($"Hasło musi mieć długość co najmniej {_settingsService.PasswordMinLength} znaków");

            if (!password.Any(char.IsDigit) && _settingsService.isEnabledPasswordRequriments)
                throw new UnauthorizedException("Hasło musi mieć co najmniej jedną cyfrę");
        }

        public async Task ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);

            if (user is null)
                throw new NotFoundException("Użytkownik nie istnieje");

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, user.Id,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            if (!(changePasswordDto.NewPassword.Length >= _settingsService.PasswordMinLength))
                throw new UnauthorizedException($"Hasło musi mieć długość co najmniej {_settingsService.PasswordMinLength} znaków");

            if (!changePasswordDto.NewPassword.Any(char.IsDigit) && _settingsService.isEnabledPasswordRequriments)
                throw new UnauthorizedException("Hasło musi mieć co najmniej jedną cyfrę");

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
                throw new ForbidException();

            if (user.IsFirstLogin == true)
                user.IsFirstLogin = false;

            user.LastPasswordChangedDate = DateTime.Now;
            await _userManager.UpdateAsync(user);
        }

        public async Task<ResponseTokenDto> AuthenticationAsync(LoginDto loginDto)
        {
            var loginCredentials = _mapper.Map<RequestTokenDto>(loginDto);

            var user = await _userManager.FindByEmailAsync(loginCredentials.Email);

            if (user is null)
                throw new UnauthorizedException("Login lub Hasło niepoprawny");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Użytkownik jest zablokowany");


            if (!await _userManager.CheckPasswordAsync(user, loginCredentials.Password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedException("Login lub Hasło niepoprawny");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles is null)
                throw new UnauthorizedException("Użytkownik nie ma przypisanej roli");

            await _userManager.ResetAccessFailedCountAsync(user);

            var t = GenerateJWT(user, userRoles[0]);

            if (user.LastPasswordChangedDate.AddDays(_settingsService.PasswordExpirationTime) < DateTime.Now)
                t.hasPasswordExpired = true;

            t.IsFirstLogin = user.IsFirstLogin;
            t.Role = userRoles[0];
            return t;

        }



        public DateTime GetTokenExpirationDate(string token)
        {
            return new JwtSecurityTokenHandler().ReadToken(token).ValidTo;
        }

        public ResponseTokenDto GenerateJWT(User userInfo, string userRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            var claims = new List<Claim>()
            {
                 new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            };

            claims.Add(new Claim(ClaimTypes.Role, userRole));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(22),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new ResponseTokenDto()
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = GetTokenExpirationDate(tokenHandler.WriteToken(token)),
            };
        }
    }
}
