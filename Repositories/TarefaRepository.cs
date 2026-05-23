using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain.Tarefas;
using trabalho2.Repositories.Interfaces;

namespace trabalho2.Repositories
{
    public class TarefaRepository : Repository<Tarefa>
    {
        private readonly ApplicationDbContext _context;

        public TarefaRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Tarefa>> RetornaTarefaPorUsuario(string userId)
        {
            return await _context.Set<Tarefa>()
                .Where(x => x.Usuario == userId)
                .ToListAsync();
        }
    }
}