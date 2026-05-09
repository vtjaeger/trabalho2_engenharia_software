using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using trabalho2.Domain.Dtos.Request;
using trabalho2.Repositories;

namespace trabalho2.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _repository;

        public AuthController(UserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var users =
                await _repository.RetornarTodosUsuarios();

            var user = users.FirstOrDefault(u => u.Email == request.Usuario && u.Situacao == "A");

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Senha, user.Senha))
            {
                return Unauthorized("Usuário ou senha inválidos" );
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes("super_secret_key_123456789_super_secret_key_123456789");

            var token = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }, 
            expires: DateTime.UtcNow.AddHours(2),

            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
                )
            );

            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString});
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout realizado");
        }
    }
}