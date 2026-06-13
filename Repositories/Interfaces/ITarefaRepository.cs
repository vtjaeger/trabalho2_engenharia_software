using trabalho2.Domain.Tarefas;
using trabalho2.Repositories.Interfaces.Base;

namespace trabalho2.Repositories.Interfaces
{
    public interface ITarefaRepository : IRepository<Tarefa>
    {
        Task<List<Tarefa>> RetornaTarefaPorUsuario(string usuario);

        /// <summary>
        /// genérico para filtro
        /// status, usuário e período de criação.
        /// </summary>
        Task<List<Tarefa>> Filtrar(string? status, string? usuario, DateTime? inicio, DateTime? fim);
    }
}
