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
            var query = _context.Set<Tarefa>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<TarefaSituacaoEnum>(status, true, out var situacao)) // transforma em string 
                {
                    query = query.Where(t => t.Situacao == situacao);
                }
            }

            if (!string.IsNullOrWhiteSpace(usuario))
            {
                query = query.Where(t => t.Usuario == usuario);
            }

            if (inicio.HasValue)
            {
                query = query.Where(t => t.InicioDataHora >= inicio.Value);
            }

            if (fim.HasValue)
            {
                query = query.Where(t => t.InicioDataHora <= fim.Value);
            }

            return await query.ToListAsync();
        }
    }
}