using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using NLog;
using System.Data;
using Microsoft.AspNetCore.StaticFiles;
using CyberbezpApi.Database.Entities;
using Microsoft.AspNetCore.Identity;
using CyberbezpApi.Models;
using CyberbezpApi.Services.Interfaces;
using CyberbezpApi.Database;

namespace CyberbezpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserContextService _userContextService;
        private readonly ApplicationDbContext _dbContext;

        public FileController(UserManager<User> userManager, IUserContextService userContextService, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _userContextService = userContextService;
            _dbContext = dbContext;
        }


        [HttpGet]
        [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
        public async Task<ActionResult> GetFileAsync([FromQuery] string fileName)
        {
            var user = await _userManager.FindByIdAsync(_userContextService.GetUserId);
            var firstDayOfMonth = new DateTime(user.FirstAccess.Year, user.FirstAccess.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            if (lastDayOfMonth < DateTime.Now || user.IsFileBlock)
            {
                user.IsFileBlock = true;
                _dbContext.SaveChanges();
                return Unauthorized("Nie masz dostępu, odblokuj za pomocą klucza Vinegret");
            }
            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{rootPath}/PrivateFiles/{fileName}";

            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return File(fileContents, contentType, fileName);
        }
    }
}
