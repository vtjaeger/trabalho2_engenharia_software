using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain;
using trabalho2.Domain.Dtos.Request;

namespace trabalho2.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<User?> RetornaUsuario(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Salvar()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> RetornarTodosUsuarios()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> CriarUsuario(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> AtualizarUsuario(User user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeletarUsuario(User user)
        {
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
