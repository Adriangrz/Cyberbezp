using CyberbezpApi.Models;
using CyberbezpApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CyberbezpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseTokenDto>> Login([FromBody] LoginDto dto)
        {
            var responseTokenData = await _authService.AuthenticationAsync(dto);

            return Ok(responseTokenData);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ResponseTokenDto>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _authService.ChangePassword(dto);

            return Ok();
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Register([FromBody] RegistrationDto dto)
        {
            await _authService.RegistrationAsync(dto);

            return Ok();
        }

        [HttpPost("EnableOrDisablePasswordRequirements")]
        [Authorize(Roles = "Admin")]
        public ActionResult EnableOrDisablePasswordRequirements([FromBody] bool isEnable)
        {
            _authService.EnableOrDisablePasswordRequirements(isEnable);

            return Ok();
        }

        [HttpPost("ChangePasswordMinLength")]
        [Authorize(Roles = "Admin")]
        public ActionResult ChangePasswordMinLength([FromBody] int minLength)
        {
            _authService.ChangePasswordMinLength(minLength);

            return Ok();
        }

        [HttpPost("SetPasswordExpirationTime")]
        [Authorize(Roles = "Admin")]
        public ActionResult SetPasswordExpirationTime([FromBody] int time)
        {
            _authService.SetPasswordExpirationTime(time);

            return Ok();
        }
    }
}
