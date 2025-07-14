using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefasController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        /// <summary>
        /// Lista todas as tarefas de um projeto.
        /// </summary>
        /// <param name="projetoId">ID do projeto.</param>
        /// <returns>Lista de tarefas do projeto.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        [HttpGet("projeto/{projetoId}")]
        [ProducesResponseType(typeof(IEnumerable<CriarTarefaDto>), 200)]
        public async Task<IActionResult> ListarPorProjeto(int projetoId)
        {
            var tarefas = await _tarefaService.ObterTodasPorProjetoAsync(projetoId);
            return Ok(tarefas);
        }

        /// <summary>
        /// Cria uma nova tarefa para o projeto especificado, limite máximo de 20 tarefas por projeto.
        /// </summary>
        /// <param name="projetoId">ID do projeto.</param>
        /// <param name="dto">Dados da tarefa a ser criada.</param>
        /// <response code="201">Tarefa criada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost("projeto/{projetoId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar(int projetoId, [FromBody] CriarTarefaDto dto)
        {
            await _tarefaService.AdicionarTarefaAsync(dto, projetoId);
            return StatusCode(201);
        }

        /// <summary>
        /// Atualiza uma tarefa existente (Status 'pendente', 'em andamento', 'concluída') Prioridade (('baixa', 'média', 'alta'))..
        /// </summary>
        /// <param name="id">ID da tarefa.</param>
        /// <param name="dto">Dados para atualização da tarefa.</param>
        /// <response code="204">Tarefa atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Tarefa não encontrada.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarTarefaDto dto)
        {
            await _tarefaService.AtualizarTarefaAsync(id, dto, 2);
            return NoContent();
        }

        /// <summary>
        /// Deleta uma tarefa pelo ID.
        /// </summary>
        /// <param name="id">ID da tarefa.</param>
        /// <response code="204">Tarefa deletada com sucesso.</response>
        /// <response code="404">Tarefa não encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            await _tarefaService.RemoverTarefaAsync(id);
            return NoContent();
        }
    }
}
