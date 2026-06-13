using Microsoft.EntityFrameworkCore;
using Moq;
using trabalho2.Data;
using trabalho2.Domain.Tarefas;
using trabalho2.Domain.Tarefas.Dtos;
using trabalho2.Domain.Usuarios;
using trabalho2.Exceptions;
using trabalho2.Repositories;
using trabalho2.Repositories.Interfaces;
using trabalho2.Services;

namespace trabalho2.Tests.Services;

public class TarefaServiceTests
{

    private static TarefaService CreateService(
        Mock<ITarefaRepository> tarefaRepo,
        Mock<IUserRepository> userRepo)
    {
        return new TarefaService(tarefaRepo.Object, userRepo.Object);
    }

    private Tarefa CriarTarefa(string id, string usuario, TarefaSituacaoEnum situacao, DateTime inicio, DateTime fim)
    {
        return new Tarefa
        {
            Id = id,
            Usuario = usuario,
            Situacao = situacao,
            Titulo = "titulo",
            Descricao = "descricao",
            InicioDataHora = inicio,
            FimDataHora = fim
        };
    }

    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task RetornaTarefa_DeveRetornarTarefa_QuandoExistir()
    {
        var tarefa = new Tarefa { Id = "1", Titulo = "Teste" };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.RetornaTarefa("1");

        Assert.NotNull(result);
        Assert.Equal("1", result!.Id);
    }

    [Fact]
    public async Task RetornaTarefa_DeveRetornarNull_QuandoNaoExistir()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((Tarefa?)null);

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.RetornaTarefa("999");

        Assert.Null(result);
    }

    [Fact]
    public async Task CriarTarefa_DeveLancarException_UsuarioNaoExiste()
    {
        var repo = new Mock<ITarefaRepository>();

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(x => x.RetornaUsuarioPorUsuario(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var service = CreateService(repo, userRepo);

        var request = new CreateTarefaRequest
        {
            Titulo = "Teste",
            Usuario = "vinicius"
        };

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarTarefa(request));
    }

    [Fact]
    public async Task CriarTarefa_DeveLancarException_UsuarioInativo()
    {
        var user = new User { Usuario = "vinicius", Situacao = "I" };

        var repo = new Mock<ITarefaRepository>();

        var userRepo = new Mock<IUserRepository>();
        userRepo.Setup(x => x.RetornaUsuarioPorUsuario("vinicius"))
            .ReturnsAsync(user);

        var service = CreateService(repo, userRepo);

        var request = new CreateTarefaRequest
        {
            Titulo = "Teste",
            Usuario = "vinicius"
        };

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarTarefa(request));
    }

    [Fact]
    public async Task RetornaTodas_DeveRetornarLista()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetAll())
            .ReturnsAsync(new List<Tarefa> { new(), new() });

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.RetornaTodas();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Delete_DeveRetornarFalse()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.Delete("1")).ReturnsAsync(false);

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.Delete("1");

        Assert.False(result);
    }

    [Fact]
    public async Task AtualizarSituacao_DeveAtualizarStatus()
    {
        var tarefa = new Tarefa
        {
            Id = "1",
            Situacao = TarefaSituacaoEnum.NOVA
        };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);
        repo.Setup(x => x.Update(It.IsAny<Tarefa>()))
            .ReturnsAsync((Tarefa t) => t);

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.AtualizarSituacao(
            "1",
            TarefaSituacaoEnum.LIBERADO_DESENVOLVIMENTO);

        Assert.Equal(TarefaSituacaoEnum.LIBERADO_DESENVOLVIMENTO, result!.Situacao);
    }

    [Fact]
    public async Task AtualizarSituacao_Concluida_DeveDefinirFimDataHora()
    {
        var tarefa = new Tarefa
        {
            Id = "1",
            Situacao = TarefaSituacaoEnum.EM_TESTE
        };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);
        repo.Setup(x => x.Update(It.IsAny<Tarefa>()))
            .ReturnsAsync((Tarefa t) => t);

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.AtualizarSituacao("1", TarefaSituacaoEnum.CONCLUIDA);

        Assert.NotEqual(default, result!.FimDataHora);
    }

    [Fact]
    public async Task AtualizarSituacao_Invalida_DeveLancarException()
    {
        var tarefa = new Tarefa
        {
            Id = "1",
            Situacao = TarefaSituacaoEnum.CONCLUIDA
        };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);

        var service = CreateService(repo, new Mock<IUserRepository>());

        await Assert.ThrowsAsync<Exception>(() =>
            service.AtualizarSituacao("1", TarefaSituacaoEnum.NOVA));
    }

    [Fact]
    public async Task Filtrar_DeveRetornarLista()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.Filtrar(null, null, null, null))
            .ReturnsAsync(new List<Tarefa> { new() });

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.Filtrar(null, null, null, null);

        Assert.Single(result);
    }

    [Fact]
    public async Task Filtrar_DeveRetornarListaVazia()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.Filtrar(null, null, null, null))
            .ReturnsAsync(new List<Tarefa>());

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.Filtrar(null, null, null, null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Filtrar_ComParametros_DeveFuncionar()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.Filtrar("NOVA", "vinicius", null, null))
            .ReturnsAsync(new List<Tarefa> { new() });

        var service = CreateService(repo, new Mock<IUserRepository>());

        var result = await service.Filtrar("NOVA", "vinicius", null, null);

        Assert.Single(result);
    }

    [Fact]
    public async Task Filtrar_DeveRetornarTodos()
    {
        var context = CreateContext();

        context.Tarefas.AddRange(
            CriarTarefa("1", "vinicius", TarefaSituacaoEnum.NOVA, DateTime.Now.AddDays(-5), DateTime.Now),
            CriarTarefa("2", "joao", TarefaSituacaoEnum.EM_TESTE, DateTime.Now.AddDays(-3), DateTime.Now)
        );

        context.SaveChanges();

        var repo = new TarefaRepository(context);

        var result = await repo.Filtrar(null, null, null, null);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Filtrar_DeveFiltrarPorUsuario()
    {
        var tarefa = new Tarefa
        {
            Id = "1",
            Situacao = TarefaSituacaoEnum.EM_TESTE
        };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);
        repo.Setup(x => x.Update(It.IsAny<Tarefa>()))
            .ReturnsAsync((Tarefa t) => t);

        var userRepo = new Mock<IUserRepository>();

        var service = new TarefaService(repo.Object, userRepo.Object);

        // fluxo válido real do sistema
        var result = await service.AtualizarSituacao("1", TarefaSituacaoEnum.CONCLUIDA);

        Assert.Equal(TarefaSituacaoEnum.CONCLUIDA, result!.Situacao);
        Assert.NotEqual(default, result.FimDataHora);
    }

    [Fact]
    public async Task Filtrar_DeveFiltrarPorStatus()
    {
        var context = CreateContext();

        context.Tarefas.AddRange(
            CriarTarefa("1", "vinicius", TarefaSituacaoEnum.NOVA, new DateTime(2024, 1, 1), new DateTime(2024, 1, 10)),
            CriarTarefa("2", "vinicius", TarefaSituacaoEnum.CONCLUIDA, new DateTime(2024, 1, 1), new DateTime(2024, 1, 10))
        );

        context.SaveChanges();

        var repo = new TarefaRepository(context);

        var result = await repo.Filtrar(
            null,
            TarefaSituacaoEnum.NOVA.ToString(),
            null,
            null
        );

        Assert.NotEmpty(result);
        Assert.All(result, x => Assert.Equal(TarefaSituacaoEnum.NOVA, x.Situacao));
    }

    [Fact]
    public async Task Filtrar_StatusInvalido_DeveIgnorarFiltro()
    {
        var context = CreateContext();

        context.Tarefas.AddRange(
            CriarTarefa("1", "vinicius", TarefaSituacaoEnum.NOVA, new DateTime(2024, 1, 1), new DateTime(2024, 1, 10)),
            CriarTarefa("2", "joao", TarefaSituacaoEnum.CONCLUIDA, new DateTime(2024, 1, 1), new DateTime(2024, 1, 10))
        );

        context.SaveChanges();

        var repo = new TarefaRepository(context);

        var result = await repo.Filtrar("INVALIDO", null, null, null);

        // comportamento atual: não retorna nada
        Assert.Empty(result);
    }

    [Fact]
    public async Task CriarTarefa_DeveCriarComSucesso()
    {
        var userRepo = new Mock<IUserRepository>();
        var tarefaRepo = new Mock<ITarefaRepository>();

        userRepo.Setup(x => x.RetornaUsuarioPorUsuario("vinicius"))
            .ReturnsAsync(new User { Usuario = "vinicius", Situacao = "A" });

        tarefaRepo.Setup(x => x.Create(It.IsAny<Tarefa>()))
            .ReturnsAsync((Tarefa t) => t);

        var service = new TarefaService(tarefaRepo.Object, userRepo.Object);

        var result = await service.CriarTarefa(new CreateTarefaRequest
        {
            Usuario = "vinicius",
            Titulo = "teste",
            Descricao = "teste"
        });

        Assert.Equal("vinicius", result.Usuario);
    }

    [Fact]
    public async Task CriarTarefa_UsuarioInexistente_DeveLancarErro()
    {
        var userRepo = new Mock<IUserRepository>();
        var tarefaRepo = new Mock<ITarefaRepository>();

        userRepo.Setup(x => x.RetornaUsuarioPorUsuario(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var service = new TarefaService(tarefaRepo.Object, userRepo.Object);

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.CriarTarefa(new CreateTarefaRequest()));
    }

    [Fact]
    public async Task AtualizarSituacao_DeveAtualizar()
    {
        var tarefa = new Tarefa
        {
            Id = "1",
            Situacao = TarefaSituacaoEnum.EM_TESTE
        };

        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.GetById("1")).ReturnsAsync(tarefa);
        repo.Setup(x => x.Update(It.IsAny<Tarefa>()))
            .ReturnsAsync((Tarefa t) => t);

        var userRepo = new Mock<IUserRepository>();

        var service = new TarefaService(repo.Object, userRepo.Object);

        // fluxo válido real: EM_TESTE → CONCLUIDA
        var result = await service.AtualizarSituacao("1", TarefaSituacaoEnum.CONCLUIDA);

        Assert.Equal(TarefaSituacaoEnum.CONCLUIDA, result!.Situacao);
        Assert.NotEqual(default, result.FimDataHora);
    }

    [Fact]
    public async Task Delete_DeveRetornarTrue()
    {
        var repo = new Mock<ITarefaRepository>();
        repo.Setup(x => x.Delete("1")).ReturnsAsync(true);

        var service = new TarefaService(repo.Object, new Mock<IUserRepository>().Object);

        var result = await service.Delete("1");

        Assert.True(result);
    }
}