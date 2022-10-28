using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using NLog;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CyberbezpApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public string[] Get()
        {
            var target = LogManager.Configuration.FindTargetByName<MemoryTarget>("information");
            IList<string> logs = target.Logs;
            return logs.ToArray();
        }
    }
}
