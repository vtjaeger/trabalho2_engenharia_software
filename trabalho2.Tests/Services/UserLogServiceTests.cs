using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trabalho2.Domain.Usuarios;
using trabalho2.Repositories.Interfaces;
using trabalho2.Services;

namespace trabalho2.Tests.Services
{
    public class UserLogServiceTests
    {
        [Fact]
        public async Task SalvarLogs_DeveCriarUmLogParaCadaCampo()
        {
            var repository = new Mock<IUserLogRepository>();
            repository.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog log) => log);
            var service = new UserLogService(repository.Object);

            var valores = new Dictionary<string, string>
            {
                { "Nome", "Vinicius" },
                { "Email", "teste@email.com" }
            };

            await service.SalvarLogs("1", valores, "admin");

            repository.Verify(x => x.Create(It.IsAny<UsuarioLog>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SalvarLogs_ComDicionarioVazio_NaoDeveCriarLogs()
        {
            var repository = new Mock<IUserLogRepository>();

            var service = new UserLogService(repository.Object);

            await service.SalvarLogs(
                "1",
                new Dictionary<string, string>(),
                "admin");

            repository.Verify(
                x => x.Create(It.IsAny<UsuarioLog>()),
                Times.Never);
        }

        [Fact]
        public async Task RetornarTodosLogs_DeveRetornarLista()
        {
            var lista = new List<UsuarioLog>
            {
                new UsuarioLog
                {
                    Id = "1",
                    Campo = "Nome"
                },
                new UsuarioLog
                {
                    Id = "2",
                    Campo = "Email"
                }
            };

            var repository = new Mock<IUserLogRepository>();
            repository.Setup(x => x.GetAll()).ReturnsAsync(lista);

            var service = new UserLogService(repository.Object);
            var result = await service.RetornarTodosLogs();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SalvarLogs_DeveCriarLogs()
        {
            var repo = new Mock<IUserLogRepository>();

            repo.Setup(x => x.Create(It.IsAny<UsuarioLog>())).ReturnsAsync((UsuarioLog l) => l);

            var service = new UserLogService(repo.Object);

            var alteracoes = new Dictionary<string, string>
            {
                { "Email", "antigo@email.com" },
                { "Nome", "Antigo Nome" }
            };

            await service.SalvarLogs("1", alteracoes, "admin");

            repo.Verify(x => x.Create(It.IsAny<UsuarioLog>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SalvarLogs_DeveChamarRepository()
        {
            var repo = new Mock<IUserLogRepository>();

            repo.Setup(x => x.Create(It.IsAny<UsuarioLog>()))
                .ReturnsAsync((UsuarioLog l) => l);

            var service = new UserLogService(repo.Object);

            var dict = new Dictionary<string, string>
    {
        { "Nome", "AntigoNome" }
    };

            await service.SalvarLogs("1", dict, "admin");

            repo.Verify(x => x.Create(It.IsAny<UsuarioLog>()), Times.Once);
        }


    }
}
