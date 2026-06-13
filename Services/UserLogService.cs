using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;

namespace trabalho2.Services
{
    public class UserLogService
    {
        private readonly IUserLogRepository _repository;

        public UserLogService(IUserLogRepository repository)
        {
            _repository = repository;
        }

        public async Task SalvarLogs(string userId, Dictionary<string, string> valoresAlterados, string userAlteracao)
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

                await _repository.Create(log);
            }
        }

        public async Task<List<UsuarioLog>> RetornarTodosLogs()
        {
            return await _repository.GetAll();
        }
    }
}