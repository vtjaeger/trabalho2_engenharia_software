using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;
using trabalho2.Repositories.Interfaces.Base;

namespace trabalho2.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
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