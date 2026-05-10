using trabalho2.Domain.Tarefas;
using trabalho2.Domain.Tarefas.Dtos;
using trabalho2.Exceptions;
using trabalho2.Repositories;

namespace trabalho2.Services
{
    public class TarefaService
    {
        private readonly TarefaRepository _repository;
        private readonly UserRepository _userRepository;

        public TarefaService(TarefaRepository repository, UserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<Tarefa?> RetornaTarefa(string id)
        {
            return await _repository.RetornaTarefaPorId(id);

        }

        public async Task<List<Tarefa>> RetornaTodas()
        {
            return await _repository.RetornaTodasTarefas();
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

            return await _repository.CriarTarefa(task);
        }

        public async Task<List<Tarefa>> RetornaPorUsuario(string usuario)
        {
            return await _repository.RetornaTarefaPorUsuario(usuario);
        }

        public async Task<Tarefa?> AtualizarSituacao(string id, TarefaSituacaoEnum novaSituacao)
        {
            var tarefa = await _repository.RetornaTarefaPorId(id);

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

            return await _repository.AtualizarTarefa(tarefa);
        }

        public async Task<bool> Delete(string id)
        {
            return await _repository.DeletarTarefa(id);
        }
    }
}