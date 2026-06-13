using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;
using trabalho2.Repositories.Interfaces.Base;

namespace trabalho2.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> RetornaUsuarioPorUsuario(string usuario);
    }
}