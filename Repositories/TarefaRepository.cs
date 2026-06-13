using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain.Tarefas;
using trabalho2.Repositories.Interfaces;
using trabalho2.Repositories.Interfaces.Base;

namespace trabalho2.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        private readonly ApplicationDbContext _context;

        public TarefaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Tarefa>> RetornaTarefaPorUsuario(string userId)
        {
            return await _context.Set<Tarefa>()
                .Where(x => x.Usuario == userId)
                .ToListAsync();
        }

        public async Task<List<Tarefa>> Filtrar(string? status, string? usuario, DateTime? inicio, DateTime? fim)
        {
            var query = _context.Tarefas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(usuario))
            {
                query = query.Where(x => x.Usuario == usuario);
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<TarefaSituacaoEnum>(status, out var statusEnum))
            {
                query = query.Where(x => x.Situacao == statusEnum);
            }

            if (inicio.HasValue)
            {
                query = query.Where(x => x.InicioDataHora >= inicio.Value);
            }

            if (fim.HasValue)
            {
                query = query.Where(x => x.FimDataHora <= fim.Value);
            }

            return await query.ToListAsync();
        }
    }
}