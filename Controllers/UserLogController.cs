using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trabalho2.Domain.Usuarios;
using trabalho2.Services;

namespace trabalho2.Controllers
{
    [ApiController]
    [Route("logs")]
    [Authorize]
    public class UserLogController : ControllerBase
    {
        private readonly UserLogService _service;

        public UserLogController(UserLogService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<List<UsuarioLog>>> RetornaTodosLogs()
        {
            var logs = await _service.RetornarTodosLogs();
            return Ok(logs);
        }
    }
}
