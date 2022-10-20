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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}/UserName")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUserName([FromBody] string name, [FromRoute] string id)
        {

            var user = await _userService.ChangeName(id, name);

            return Ok(user);
        }
        [HttpPut("{id}/Password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdatePassword([FromBody] string password, [FromRoute] string id)
        {

            await _userService.ChangePassword(id, password);

            return Ok();
        }

        [HttpPost("{id}/BlockUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> BlockUser([FromRoute] string id)
        {

            await _userService.BlockUser(id);

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userService.GetUsers();

            return Ok(users);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> Delete([FromRoute] string id)
        {
            await _userService.DeleteUser(id);

            return Ok(id);
        }
    }
}
