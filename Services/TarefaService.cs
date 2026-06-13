using trabalho2.Domain.Tarefas;
using trabalho2.Domain.Tarefas.Dtos;
using trabalho2.Exceptions;
using trabalho2.Repositories;
using trabalho2.Repositories.Interfaces;

namespace trabalho2.Services
{
    public class TarefaService
    {
        private readonly ITarefaRepository _repository;
        private readonly IUserRepository _userRepository;

        public TarefaService(ITarefaRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Tarefa?> RetornaTarefa(string id)
        {
            return await _repository.GetById(id);
        }

        public async Task<List<Tarefa>> RetornaTodas()
        {
            return await _repository.GetAll();
        }

        public async Task<Tarefa> CriarTarefa(CreateTarefaRequest request)
        {
            var usuario = await _userRepository.RetornaUsuarioPorUsuario(request.Usuario);

            if (usuario == null)
                throw new BusinessException("Usuário não encontrado");

            if (usuario.Situacao == "I")
                throw new BusinessException("Usuário inativo");

            var task = new Tarefa
            {
                Id = Guid.NewGuid().ToString(),
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Usuario = usuario.Usuario,
                Situacao = TarefaSituacaoEnum.NOVA,
                InicioDataHora = DateTime.Now
            };

            return await _repository.Create(task);
        }

        public async Task<List<Tarefa>> RetornaPorUsuario(string usuario)
        {
            return await _repository.RetornaTarefaPorUsuario(usuario);
        }

        public async Task<Tarefa?> AtualizarSituacao(string id, TarefaSituacaoEnum novaSituacao)
        {
            var tarefa = await _repository.GetById(id);

            if (tarefa == null)
                return null;

            var atual = tarefa.Situacao;

            bool valido = (atual == TarefaSituacaoEnum.NOVA && novaSituacao == TarefaSituacaoEnum.LIBERADO_DESENVOLVIMENTO) ||
                (atual == TarefaSituacaoEnum.LIBERADO_DESENVOLVIMENTO && novaSituacao == TarefaSituacaoEnum.EM_DESENVOLVIMENTO) ||
                (atual == TarefaSituacaoEnum.EM_DESENVOLVIMENTO && novaSituacao == TarefaSituacaoEnum.EM_TESTE) ||
                (atual == TarefaSituacaoEnum.EM_TESTE && novaSituacao == TarefaSituacaoEnum.CONCLUIDA);

            if (!valido)
                throw new Exception($"Transição inválida: {atual} → {novaSituacao}");

            tarefa.Situacao = novaSituacao;

            if (novaSituacao == TarefaSituacaoEnum.CONCLUIDA)
                tarefa.FimDataHora = DateTime.Now;

            return await _repository.Update(tarefa);
        }

        public async Task<bool> Delete(string id)
        {
            return await _repository.Delete(id);
        }

        public async Task<List<Tarefa>> Filtrar(string? status, string? usuario, DateTime? inicio, DateTime? fim)
        {
            return await _repository.Filtrar(status, usuario, inicio, fim);
        }
    }
}