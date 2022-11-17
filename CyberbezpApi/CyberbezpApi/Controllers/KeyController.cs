using CyberbezpApi.Database;
using CyberbezpApi.Database.Entities;
using CyberbezpApi.Exceptions;
using CyberbezpApi.Models;
using CyberbezpApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CyberbezpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyController : ControllerBase
    {
		private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserContextService _userContextService;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

		public KeyController(ApplicationDbContext applicationDbContext, IUserContextService userContextService, ApplicationDbContext dbContext, UserManager<User> userManager)
		{
			_applicationDbContext = applicationDbContext;
			_userContextService = userContextService;
			_dbContext = dbContext;
			_userManager = userManager;
		}
		private static int Mod(int a, int b)
        {
            return (a % b + b) % b;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> CreateKey(string input, string key)
		{
			var dbKey = new Key()
			{
				value = input
			};
			_applicationDbContext.Keys.Add(dbKey);
			_applicationDbContext.SaveChanges();
			var unlock = false;
			for (int i = 0; i < key.Length; ++i)
				if (!char.IsLetter(key[i]))
					return null; // Error

			string output = string.Empty;
			int nonAlphaCharCount = 0;

			for (int i = 0; i < input.Length; ++i)
			{
				if (char.IsLetter(input[i]))
				{
					bool cIsUpper = char.IsUpper(input[i]);
					char offset = cIsUpper ? 'A' : 'a';
					int keyIndex = (i - nonAlphaCharCount) % key.Length;
					int k = (cIsUpper ? char.ToUpper(key[keyIndex]) : char.ToLower(key[keyIndex])) - offset;
					k = unlock ? k : -k;
					char ch = (char)((Mod(((input[i] + k) - offset), 26)) + offset);
					output += ch;
				}
				else
				{
					output += input[i];
					++nonAlphaCharCount;
				}
			}

			return output;
		}

		[HttpGet]
		public async Task<ActionResult> Unlock(string input, string key)
		{
			var unlock = true;
			for (int i = 0; i < key.Length; ++i)
				if (!char.IsLetter(key[i]))
					return null; // Error

			string output = string.Empty;
			int nonAlphaCharCount = 0;

			for (int i = 0; i < input.Length; ++i)
			{
				if (char.IsLetter(input[i]))
				{
					bool cIsUpper = char.IsUpper(input[i]);
					char offset = cIsUpper ? 'A' : 'a';
					int keyIndex = (i - nonAlphaCharCount) % key.Length;
					int k = (cIsUpper ? char.ToUpper(key[keyIndex]) : char.ToLower(key[keyIndex])) - offset;
					k = unlock ? k : -k;
					char ch = (char)((Mod(((input[i] + k) - offset), 26)) + offset);
					output += ch;
				}
				else
				{
					output += input[i];
					++nonAlphaCharCount;
				}
			}
			var dbKey = _applicationDbContext.Keys.FirstOrDefault(k => k.value == output);
			if (dbKey is not null)
			{
                var user = await _userManager.FindByIdAsync(_userContextService.GetUserId);
				user.IsFileBlock = false;
				var date = new DateTime(user.FirstAccess.Year, user.FirstAccess.Month, 1);
				date = date.AddMonths(1);
				user.FirstAccess = date;
				_dbContext.SaveChanges();
                return Ok();
			}
			else
				throw new UnauthorizedException("Zły klucz");
		}
    }
}
