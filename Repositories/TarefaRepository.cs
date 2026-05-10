using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain.Tarefas;

namespace trabalho2.Repositories
{
    public class TarefaRepository
    {
        private readonly ApplicationDbContext _context;

        public TarefaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tarefa?> RetornaTarefaPorId(string id)
        {
            return await _context.Set<Tarefa>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Tarefa>> RetornaTarefaPorUsuario(string userId)
        {
            return await _context.Set<Tarefa>().Where(x => x.Usuario == userId).ToListAsync();
        }

        public async Task<List<Tarefa>> RetornaTodasTarefas()
        {
            return await _context.Set<Tarefa>().ToListAsync();
        }

        public async Task<Tarefa> CriarTarefa(Tarefa task)
        {
            _context.Set<Tarefa>().Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<Tarefa> AtualizarTarefa(Tarefa task)
        {
            _context.Set<Tarefa>().Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeletarTarefa(string id)
        {
            var task = await RetornaTarefaPorId(id);

            if (task == null)
                return false;

            _context.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}