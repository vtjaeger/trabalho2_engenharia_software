using trabalho2.Data;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;
using trabalho2.Repositories.Interfaces.Base;

namespace trabalho2.Repositories
{
    public class UserLogRepository : Repository<UsuarioLog>, IUserLogRepository
    {
        public UserLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}