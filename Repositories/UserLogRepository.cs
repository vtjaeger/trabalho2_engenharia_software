using trabalho2.Data;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;

namespace trabalho2.Repositories
{
    public class UserLogRepository : Repository<UsuarioLog>
    {
        public UserLogRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}