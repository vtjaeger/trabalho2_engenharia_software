using Microsoft.EntityFrameworkCore;
using Moq;
using trabalho2.Data;
using trabalho2.Domain.Usuarios;
using trabalho2.Domain.Usuarios.Dtos;
using trabalho2.Exceptions;
using trabalho2.Repositories;
using trabalho2.Repositories.Interfaces;
using trabalho2.Services;
using Xunit;

namespace trabalho2.Tests.Services;

public class UserServiceTests
{
    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private (UserService service, Mock<IUserRepository> repo, Mock<IUserLogRepository> logRepo)CreateUserService()
    {
        var repo = new Mock<IUserRepository>();
        var logRepo = new Mock<IUserLogRepository>();

        var logService = new UserLogService(logRepo.Object);
        var service = new UserService(repo.Object, logService);

        return (service, repo, logRepo);
    }

    [Fact]
    public async Task CriarUsuario_DeveCriarComSucesso()
    {
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
        userRepository.Setup(x => x.Create(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123456",
            Role = UserRole.ALUNO
        };

        var result = await service.CriarUsuario(user);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.Id));
        Assert.Equal("A", result.Situacao);
        Assert.Equal("teste@email.com", result.Email);
        Assert.NotEqual("123456", result.Senha);
    }

    [Fact]
    public async Task RetornaUsuario_DeveRetornarUsuario_QuandoExistir()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123456",
            Situacao = "A",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);
        var result = await service.RetornaUsuario("1");

        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("Vinicius", result.Nome);
    }

    [Fact]
    public async Task RetornaUsuario_DeveLancarBusinessException_QuandoNaoExistir()
    {
        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((User?)null);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        await Assert.ThrowsAsync<BusinessException>(() => service.RetornaUsuario("1"));
    }

    [Fact]
    public async Task CriarUsuario_DeveLancarBusinessException_QuandoUsuarioForNull()
    {
        var userRepository = new Mock<IUserRepository>();

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        await Assert.ThrowsAsync<BusinessException>(() => service.CriarUsuario(null!));
    }

    [Fact]
    public async Task CriarUsuario_DeveLancarBusinessException_QuandoEmailForVazio()
    {
        var userRepository = new Mock<IUserRepository>();

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "",
            Senha = "123456"
        };

        await Assert.ThrowsAsync<BusinessException>(() => service.CriarUsuario(user));
    }

    [Fact]
    public async Task CriarUsuario_DeveLancarBusinessException_QuandoUsuarioForVazio()
    {
        var userRepository = new Mock<IUserRepository>();

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "",
            Email = "teste@email.com",
            Senha = "123456"
        };

        await Assert.ThrowsAsync<BusinessException>(() => service.CriarUsuario(user));
    }

    [Fact]
    public async Task CriarUsuario_DeveLancarBusinessException_QuandoEmailJaExistir()
    {
        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetAll()).ReturnsAsync(
            new List<User>
            {
                new User
                {
                    Email = "teste@email.com"
                }
            });

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123456"
        };

        await Assert.ThrowsAsync<BusinessException>(() => service.CriarUsuario(user));
    }

    [Fact]
    public async Task RetornaTodosUsuarios_DeveRetornarLista()
    {
        var lista = new List<User>
        {
            new User { Nome = "A" },
            new User { Nome = "B" }
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetAll()).ReturnsAsync(lista);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var result = await service.RetornaTodosUsuarios();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task RetornaUsuariosAtivos_DeveRetornarSomenteUsuariosAtivos()
    {
        var lista = new List<User>
        {
            new User { Nome = "A", Situacao = "A" },
            new User { Nome = "B", Situacao = "I" },
            new User { Nome = "C", Situacao = "A" }
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetAll()).ReturnsAsync(lista);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var result = await service.RetornaUsuariosAtivos();

        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.Equal("A", u.Situacao));
    }

    [Fact]
    public async Task AlterarSituacaoUsuario_DeveLancarBusinessException_QuandoUsuarioNaoExistir()
    {
        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((User?)null);

        var logRepository = new Mock<IUserLogRepository>();

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        await Assert.ThrowsAsync<BusinessException>(() => service.AlterarSituacaoUsuario("1", "admin"));
    }

    [Fact]
    public async Task AlterarSituacaoUsuario_DeveAlterarParaInativo()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Situacao = "A"
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById("1"))
            .ReturnsAsync(user);

        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();

        logRepository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var result = await service.AlterarSituacaoUsuario("1", "admin");

        Assert.NotNull(result);
        Assert.Equal("I", result!.Situacao);

        userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task AlterarSituacaoUsuario_DeveAlterarParaAtivo()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Situacao = "I"
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);

        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();

        logRepository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var result = await service.AlterarSituacaoUsuario("1", "admin");

        Assert.Equal("A", result!.Situacao);
    }

    [Fact]
    public async Task AtualizarUsuario_DeveLancarBusinessException_QuandoUsuarioNaoExistir()
    {
        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((User?)null);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var request = new UpdateUserRequest();

        await Assert.ThrowsAsync<BusinessException>(() => service.AtualizarUsuario("1", request, "admin"));
    }

    [Fact]
    public async Task AtualizarUsuario_DeveAtualizarNome()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);

        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();

        logRepository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var request = new UpdateUserRequest
        {
            Usuario = "Pedro"
        };

        var result = await service.AtualizarUsuario("1", request, "admin");

        Assert.NotNull(result);
        Assert.Equal("Pedro", result!.Nome);

        userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarUsuario_DeveAtualizarEmail()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);

        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();

        logRepository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var request = new UpdateUserRequest
        {
            Email = "novo@email.com"
        };

        var result = await service.AtualizarUsuario("1", request, "admin");

        Assert.NotNull(result);
        Assert.Equal("novo@email.com", result!.Email);
    }

    [Fact]
    public async Task AtualizarUsuario_DeveAtualizarRole()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();

        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);

        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();

        logRepository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);

        var request = new UpdateUserRequest
        {
            Role = UserRole.ADMIN
        };

        var result = await service.AtualizarUsuario("1", request, "admin");

        Assert.NotNull(result);
        Assert.Equal(UserRole.ADMIN, result!.Role);
    }

    [Fact]
    public async Task AtualizarUsuario_DeveManterValoresQuandoRequestVazia()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);
        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);

        var service = new UserService(userRepository.Object, logService);
        var request = new UpdateUserRequest();

        var result = await service.AtualizarUsuario("1", request, "admin");
        Assert.Equal("Vinicius", result!.Nome);
        Assert.Equal("teste@email.com", result.Email);
        Assert.Equal(UserRole.ALUNO, result.Role);
    }

    [Fact]
    public async Task AtualizarUsuario_DeveChamarUpdate()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(x => x.GetById("1")).ReturnsAsync(user);
        userRepository.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepository = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepository.Object);
        var service = new UserService(userRepository.Object, logService);

        await service.AtualizarUsuario("1",
            new UpdateUserRequest
            {
                Email = "novo@email.com"
            }, "admin");

        userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RetornaUsuarioPorUsuario_DeveRetornarUsuario()
    {
        var context = CreateContext();

        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123456",
            Situacao = "A",
            Role = UserRole.ALUNO
        };

        context.Users.Add(user);   // ✅ FALTAVA ISSO
        context.SaveChanges();     // agora sim

        var repo = new UserRepository(context);

        var result = await repo.RetornaUsuarioPorUsuario("vinicius");

        Assert.NotNull(result);
        Assert.Equal("vinicius", result!.Usuario);
    }

    [Fact]
    public async Task AtualizarUsuario_SemMudancas_NaoDeveAlterarNada()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepo = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepo.Object);

        var service = new UserService(repo.Object, logService);

        var result = await service.AtualizarUsuario("1", new UpdateUserRequest(), "admin");

        Assert.Equal("Vinicius", result!.Nome);
        Assert.Equal("teste@email.com", result.Email);
    }

    [Fact]
    public async Task AtualizarUsuario_SoEmail_DeveAtualizar()
    {
        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "old@email.com"
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepo = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepo.Object);

        var service = new UserService(repo.Object, logService);

        var result = await service.AtualizarUsuario("1",
            new UpdateUserRequest { Email = "new@email.com" }, "admin");

        Assert.Equal("new@email.com", result!.Email);
    }

    [Fact]
    public async Task AtualizarUsuario_SoRole_DeveAtualizar()
    {
        var user = new User
        {
            Id = "1",
            Role = UserRole.ALUNO
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepo = new Mock<IUserLogRepository>();
        var logService = new UserLogService(logRepo.Object);

        var service = new UserService(repo.Object, logService);

        var result = await service.AtualizarUsuario("1",
            new UpdateUserRequest { Role = UserRole.ADMIN }, "admin");

        Assert.Equal(UserRole.ADMIN, result!.Role);
    }

    [Fact]
    public async Task CriarUsuario_EmailDuplicado_DeveLancarException()
    {
        var (service, repo, logRepo) = CreateUserService();

        repo.Setup(x => x.GetAll()).ReturnsAsync(new List<User>
    {
        new User { Email = "teste@email.com" }
    });

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123"
        };

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarUsuario(user));
    }

    [Fact]
    public async Task AlterarSituacao_DeveAlternarAparaI()
    {
        var user = new User
        {
            Id = "1",
            Situacao = "A"
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var logRepo = new Mock<IUserLogRepository>();
        logRepo.Setup(x => x.Create(It.IsAny<UsuarioLog>()))
            .ReturnsAsync((UsuarioLog l) => l);

        var logService = new UserLogService(logRepo.Object);

        var service = new UserService(repo.Object, logService);

        var result = await service.AlterarSituacaoUsuario("1", "admin");

        Assert.Equal("I", result!.Situacao);
    }

    [Fact]
    public async Task CriarUsuario_NomeVazio_DeveLancarException()
    {
        var (service, repo, logRepo) = CreateUserService();

        var user = new User
        {
            Nome = "",
            Usuario = "vinicius",
            Email = "teste@email.com",
            Senha = "123"
        };

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarUsuario(user));
    }

    [Fact]
    public async Task CriarUsuario_UsuarioVazio_DeveLancarException()
    {
        var (service, repo, logRepo) = CreateUserService();

        var user = new User
        {
            Nome = "Vinicius",
            Usuario = "",
            Email = "teste@email.com",
            Senha = "123"
        };

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarUsuario(user));
    }

    [Fact]
    public async Task RetornaUsuario_NaoExistente_DeveLancarException()
    {
        var (service, repo, logRepo) = CreateUserService();

        repo.Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.RetornaUsuario("1"));
    }

    [Fact]
    public async Task AtualizarUsuario_RequestVazio_NaoDeveAlterarNada()
    {
        var (service, repo, logRepo) = CreateUserService();

        var user = new User
        {
            Id = "1",
            Nome = "Vinicius",
            Email = "teste@email.com",
            Role = UserRole.ALUNO
        };

        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await service.AtualizarUsuario("1", new UpdateUserRequest(), "admin");

        Assert.Equal("Vinicius", result!.Nome);
        Assert.Equal("teste@email.com", result.Email);
    }

    [Fact]
    public async Task AlterarSituacao_UsuarioNaoExiste_DeveLancarException()
    {
        var (service, repo, logRepo) = CreateUserService();

        repo.Setup(x => x.GetById(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.AlterarSituacaoUsuario("1", "admin"));
    }

    [Fact]
    public async Task AlterarSituacao_DeAParaI_EDeveSalvar()
    {
        var (service, repo, logRepo) = CreateUserService();

        var user = new User { Id = "1", Situacao = "A" };

        repo.Setup(x => x.GetById("1")).ReturnsAsync(user);
        repo.Setup(x => x.Update(It.IsAny<User>())).ReturnsAsync((User u) => u);

        var result = await service.AlterarSituacaoUsuario("1", "admin");

        Assert.Equal("I", result!.Situacao);
    }
}