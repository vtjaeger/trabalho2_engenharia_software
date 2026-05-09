using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using trabalho2.Domain;
using trabalho2.Domain.Dtos.Request;
using trabalho2.Services;

namespace trabalho2.Controllers
{
    [ApiController]
    [Route("users")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _service.RetornaTodosUsuarios();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await _service.RetornaUsuario(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("/ativos")]
        public async Task<ActionResult<List<User>>> GetAtivos()
        {
            var users = await _service.RetornaUsuariosAtivos();

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpPost]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<User>> CriarUsuario(CreateUserRequest dto)
        {
            var user = new User
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Role = UserRole.USER,
                Senha = dto.Senha
            };

            var createdUser = await _service.CriarUsuario(user);

            return Ok(createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> AtualizarUsuario(string id, [FromBody] UpdateUserRequest request)
        {
            var userAlteracao = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = await _service.AtualizarUsuario(id, request, userAlteracao);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            return Ok(user);
        }

        #region OLD
        //[HttpPut("alterar-situacao/{id}")]
        //[Authorize(Roles = "ADMIN")]
        //public async Task<ActionResult<User>> AlterarSituacao(string id)
        //{
        //    var user = await _service.AlterarSituacaoUsuario(id);

        //    if (user == null)
        //    {
        //        return NotFound("Usuário não encontrado");
        //    }

        //    return Ok(user);
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> DeletarUsuario(string id)
        //{
        //    var deleted = await _service.DeletarUsuario(id);

        //    if (!deleted)
        //    {
        //        return NotFound("Usuário não encontrado");
        //    }

        //    return NoContent();
        //}
        #endregion
    }
}