using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;

namespace trabalho2.Repositories
{
    public class UserRepository : Repository<User>
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<User?> RetornaUsuarioPorUsuario(string usuario)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == usuario);
        }
    }
}