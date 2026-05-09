using Microsoft.EntityFrameworkCore;
using trabalho2.Domain;
using trabalho2.Domain.Dtos.Request;
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
            var users = await _repository.RetornarTodosUsuarios();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<List<User>> RetornaTodosUsuarios()
        {
            return await _repository.RetornarTodosUsuarios();
        }

        public async Task<List<User?>> RetornaUsuariosAtivos()
        {
            var users = await _repository.RetornarTodosUsuarios();

            return users.Where(u => u.Situacao == "A").ToList();
        }

        public async Task<User> CriarUsuario(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.DataCadastro = DateTime.Now;
            user.Senha = BCrypt.Net.BCrypt.HashPassword(user.Senha);
            user.Situacao = "A";

            return await _repository.CriarUsuario(user);
        }

        public async Task<User?> AlterarSituacaoUsuario(string id) // soft delete
        {
            var user = await _repository.RetornaUsuario(id);

            if (user == null)
            {
                return null;
            }

            var changes = new Dictionary<string, string>();
            changes.Add("Situacao", user.Situacao);

            user.Situacao = user.Situacao == "A" ? "I" : "A";

            await _repository.Salvar();
            await _logService.SalvarLogs(user.Id, changes, "admin");

            return user;
        }

        public async Task<User?> AtualizarUsuario(string id, UpdateUserRequest request, string userAlteracao)
        {
            var user = await _repository.RetornaUsuario(id);

            if (user == null)
                return null;

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

        //public async Task<bool> DeletarUsuario(string id)
        //{
        //    var user = await _repository.RetornaUsuario(id);

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    return await _repository.DeletarUsuario(user);
        //}
    }
}
