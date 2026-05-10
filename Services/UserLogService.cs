using System.Text.Json;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories;

namespace trabalho2.Services
{
    public class UserLogService
    {
        private readonly UserLogRepository _repository;

        public UserLogService(UserLogRepository repository)
        {
            _repository = repository;
        }

        public async Task SalvarLogs(string userId, Dictionary<string, string> valoresAlterados, string? userAlteracao)
        {
            foreach (var item in valoresAlterados)
            {
                var log = new UsuarioLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UsuarioId = userId,
                    Campo = item.Key,
                    ValorAntigo = item.Value,
                    UsuarioAlteracao = userAlteracao,
                    DataHora = DateTime.Now
                };

                await _repository.AddAsync(log);
            }
        }

        public async Task<List<UsuarioLog>> RetornarTodosLogs()
        {
            return await _repository.GetAllAsync();
        }
    }
}
