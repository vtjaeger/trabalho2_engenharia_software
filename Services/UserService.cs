using Microsoft.EntityFrameworkCore;
using trabalho2.Domain.Usuarios;
using trabalho2.Domain.Usuarios.Dtos;
using trabalho2.Exceptions;
using trabalho2.Repositories;

namespace trabalho2.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly UserLogService _logService;

        public UserService(UserRepository repository, UserLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<User> RetornaUsuario(string id)
        {
            var user = await _repository.RetornaUsuarioPorId(id);

            if (user == null)
            {
                throw new BusinessException("Usuário não encontrado");
            }

            return user;
        }

        public async Task<List<User>> RetornaTodosUsuarios()
        {
            return await _repository.RetornarTodosUsuarios();
        }

        public async Task<List<User>> RetornaUsuariosAtivos()
        {
            var users = await _repository.RetornarTodosUsuarios();
            return users.Where(u => u.Situacao == "A").ToList();
        }

        public async Task<User> CriarUsuario(User user)
        {
            if (user == null)
                throw new BusinessException("Usuário inválido");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new BusinessException("Email é obrigatório");

            if (string.IsNullOrWhiteSpace(user.Usuario))
                throw new BusinessException("Usuário é obrigatório");

            var existe = (await _repository.RetornarTodosUsuarios())
                .Any(u => u.Email == user.Email);

            if (existe)
                throw new BusinessException("Já existe um usuário com esse email");

            user.Id = Guid.NewGuid().ToString();
            user.DataCadastro = DateTime.Now;
            user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);
            user.Situacao = "A";

            return await _repository.CriarUsuario(user);
        }

        public async Task<User?> AlterarSituacaoUsuario(string id) // soft delete
        {
            var user = await _repository.RetornaUsuarioPorId(id);

            if (user == null)
                throw new BusinessException("Usuário não encontrado");

            var valoresAlterados = new Dictionary<string, string>();
            valoresAlterados.Add("Situacao", user.Situacao);

            user.Situacao = user.Situacao == "A" ? "I" : "A";

            await _repository.Salvar();
            await _logService.SalvarLogs(user.Id, valoresAlterados, "admin");

            return user;
        }

        public async Task<User?> AtualizarUsuario(string id, UpdateUserRequest request, string userAlteracao)
        {
            var user = await _repository.RetornaUsuarioPorId(id);

            if (user == null)
                throw new BusinessException("Usuário não encontrado");

            var valoresAlterados = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(request.Usuario) && request.Usuario != user.Nome)
            {
                valoresAlterados.Add("Name", user.Nome ?? "");
                user.Nome = request.Usuario;
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                valoresAlterados.Add("Email", user.Email ?? "");
                user.Email = request.Email;
            }

            if (request.Role.HasValue && request.Role.Value != user.Role)
            {
                valoresAlterados.Add("Role", user.Role.ToString());
                user.Role = request.Role.Value;
            }

            await _repository.AtualizarUsuario(user);

            if (valoresAlterados.Any())
            {
                await _logService.SalvarLogs(user.Id, valoresAlterados, userAlteracao);
            }

            return user;
        }
    }
}
