using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using trabalho2.Domain.Tarefas;
using trabalho2.Services;
using trabalho2.Domain.Tarefas.Dtos;

namespace trabalho2.Controllers
{
    [ApiController]
    [Route("tasks")]
    [Authorize]
    public class TarefaController : ControllerBase
    {
        private readonly TarefaService _service;

        public TarefaController(TarefaService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> RetornaPorId(string id)
        {
            var task = await _service.RetornaTarefa(id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet]
        public async Task<ActionResult<List<Tarefa>>> RetornaTarefas()
        {
            var usuario = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(usuario))
                return Unauthorized();

            if (User.IsInRole("ADMIN"))
            {
                var todas = await _service.RetornaTodas();

                return Ok(todas);
            }

            var minhas = await _service.RetornaPorUsuario(usuario);

            return Ok(minhas);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Tarefa>> CriarTarefa(CreateTarefaRequest request)
        {
            var task = await _service.CriarTarefa(request);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Excluir(string id)
        {
            var ok = await _service.Delete(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/situacao")]
        public async Task<ActionResult<Tarefa>> AtualizarSituacao(string id, TarefaSituacaoEnum novaSituacao)
        {
            try
            {
                var tarefa = await _service.AtualizarSituacao(id, novaSituacao);

                if (tarefa == null)
                    return NotFound();

                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}