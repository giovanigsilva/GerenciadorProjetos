using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;
        private readonly ILogger<TarefasController> _logger;

        public TarefasController(ITarefaService tarefaService, ILogger<TarefasController> logger)
        {
            _tarefaService = tarefaService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas as tarefas de um projeto.
        /// </summary>
        [HttpGet("projeto/{projetoId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ListarPorProjeto(int projetoId)
        {
            try
            {
                var tarefas = await _tarefaService.ObterTodasPorProjetoAsync(projetoId);

                if (tarefas == null || !tarefas.Any())
                {
                    var msg = "Nenhuma tarefa encontrada para este projeto.";
                    _logger.LogInformation(msg + " ProjetoId: {ProjetoId}", projetoId);
                    return Ok(new { message = msg, tarefas = new List<CriarTarefaDto>() });
                }

                _logger.LogInformation("Tarefas listadas com sucesso para o projeto {ProjetoId}", projetoId);
                return Ok(new { message = MensagemApi.ListaSucesso.GetMensagem(), tarefas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar tarefas do projeto {ProjetoId}", projetoId);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Cria uma nova tarefa para um projeto (máx 20 tarefas).
        /// </summary>
        [HttpPost("projeto/{projetoId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar(int projetoId, [FromBody] CriarTarefaDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de criação de tarefa com dados nulos.");
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                await _tarefaService.AdicionarTarefaAsync(dto, projetoId);
                _logger.LogInformation("Tarefa criada com sucesso para o projeto {ProjetoId}", projetoId);
                return StatusCode(201, new { message = MensagemApi.CriacaoSucesso.GetMensagem() });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao criar tarefa. ProjetoID: {ProjetoId}", projetoId);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao criar tarefa. ProjetoID: {ProjetoId}", projetoId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar tarefa. ProjetoID: {ProjetoId}", projetoId);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Atualiza uma tarefa existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarTarefaDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de atualização com DTO nulo. ID: {Id}", id);
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                // usuárioId = 2 (fixo para exemplo)
                await _tarefaService.AtualizarTarefaAsync(id, dto, 2);
                _logger.LogInformation("Tarefa atualizada com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.AtualizacaoSucesso.GetMensagem() });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tarefa não encontrada ao tentar atualizar. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Regra de negócio falhou ao atualizar tarefa. ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar tarefa. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Exclui uma tarefa pelo ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _tarefaService.RemoverTarefaAsync(id);
                _logger.LogInformation("Tarefa removida com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.RemocaoSucesso.GetMensagem() });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tarefa não encontrada ao tentar excluir. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir tarefa. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }
    }
}
