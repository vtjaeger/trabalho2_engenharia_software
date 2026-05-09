using Microsoft.AspNetCore.Mvc;
using trabalho2.Domain;
using trabalho2.Services;

namespace trabalho2.Controllers
{
    [ApiController]
    [Route("logs")]

    public class UserLogController : ControllerBase
    {
        private readonly UserLogService _service;

        public UserLogController(UserLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserLog>>> GetAll()
        {
            var logs = await _service.GetLogs();
            return Ok(logs);
        }
    }
}
